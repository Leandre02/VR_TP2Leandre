#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class ObjectLoggerConfig : ScriptableObject
{
    public GameObject targetObject1;
    public GameObject targetObject2;
    public bool logTransform = true;
    public bool logAnimator = true;
    public bool logComponents = true;
    public bool logCollisions = true;
    public bool logTriggers = true;
    public bool logEnabled = true;
    public float logInterval = 0.1f;
    public bool logOnlyChanges = false;
    public bool useSeparateFiles = false; // Option pour fichiers séparés ou fichier unique
}

public class RuntimeObjectLogger : EditorWindow
{
    static ObjectLoggerConfig config;
    static StreamWriter logWriterCombined;
    static StreamWriter logWriter1;
    static StreamWriter logWriter2;
    
    static GameObject runtimeTarget1;
    static GameObject runtimeTarget2;
    
    static float lastLogTime;
    static bool isLogging = false;
    
    // Cache pour détecter changements - Object 1
    static ObjectCache cache1 = new ObjectCache();
    static ObjectCache cache2 = new ObjectCache();
    
    Vector2 scrollPos;
    
    // Classe pour stocker le cache d'un objet
    class ObjectCache
    {
        public Vector3 lastPosition;
        public Quaternion lastRotation;
        public Vector3 lastScale;
        public Dictionary<string, object> lastValues = new Dictionary<string, object>();
        public int lastStateHash;
        public string lastStateName;
        public Dictionary<string, float> lastAnimatorParams = new Dictionary<string, float>();
    }
    
    [MenuItem("Tools/Runtime Object Logger (Multi)")]
    public static void ShowWindow()
    {
        GetWindow<RuntimeObjectLogger>("Runtime Logger Multi");
    }
    
    void OnEnable()
    {
        if (config == null)
        {
            config = CreateInstance<ObjectLoggerConfig>();
            config.logTransform = true;
            config.logAnimator = true;
            config.logComponents = true;
            config.logCollisions = true;
            config.logTriggers = true;
            config.logEnabled = true;
            config.logInterval = 0.1f;
            config.logOnlyChanges = false;
            config.useSeparateFiles = false;
        }
        
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }
    
    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        StopLogging();
    }
    
    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        GUILayout.Label("Runtime Object Logger - Multi Objects", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        // Status
        if (isLogging)
        {
            EditorGUILayout.HelpBox("🔴 LOGGING EN COURS...", MessageType.Warning);
        }
        else if (EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("▶️ En Play Mode (prêt à logger)", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("⏸️ En Edit Mode (sélectionne tes objets puis lance Play)", MessageType.Info);
        }
        
        GUILayout.Space(10);
        
        // Sélection objets
        GUILayout.Label("Objets à logger:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("🚁 Objet 1 (ex: Drone)", EditorStyles.miniLabel);
        config.targetObject1 = EditorGUILayout.ObjectField("Target 1", config.targetObject1, typeof(GameObject), true) as GameObject;
        if (config.targetObject1 != null)
        {
            EditorGUILayout.HelpBox($"✅ Objet 1: {config.targetObject1.name}", MessageType.None);
        }
        EditorGUILayout.EndVertical();
        
        GUILayout.Space(5);
        
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("🔫 Objet 2 (ex: Tank)", EditorStyles.miniLabel);
        config.targetObject2 = EditorGUILayout.ObjectField("Target 2", config.targetObject2, typeof(GameObject), true) as GameObject;
        if (config.targetObject2 != null)
        {
            EditorGUILayout.HelpBox($"✅ Objet 2: {config.targetObject2.name}", MessageType.None);
        }
        EditorGUILayout.EndVertical();
        
        GUILayout.Space(10);
        
        // Options
        GUILayout.Label("Options de logging:", EditorStyles.boldLabel);
        config.logTransform = EditorGUILayout.Toggle("  Transform (Pos/Rot/Scale)", config.logTransform);
        config.logAnimator = EditorGUILayout.Toggle("  Animator (États + Params)", config.logAnimator);
        config.logComponents = EditorGUILayout.Toggle("  Components (Enabled/Disabled)", config.logComponents);
        config.logCollisions = EditorGUILayout.Toggle("  Collisions", config.logCollisions);
        config.logTriggers = EditorGUILayout.Toggle("  Triggers", config.logTriggers);
        config.logEnabled = EditorGUILayout.Toggle("  Enabled/Disabled events", config.logEnabled);
        
        GUILayout.Space(5);
        config.logInterval = EditorGUILayout.Slider("Intervalle (secondes)", config.logInterval, 0.01f, 1f);
        config.logOnlyChanges = EditorGUILayout.Toggle("Log seulement si changements", config.logOnlyChanges);
        config.useSeparateFiles = EditorGUILayout.Toggle("Fichiers séparés par objet", config.useSeparateFiles);
        
        if (config.useSeparateFiles)
        {
            EditorGUILayout.HelpBox("Créera 2 fichiers séparés (un par objet)", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Créera 1 fichier combiné avec les 2 objets", MessageType.Info);
        }
        
        GUILayout.Space(10);
        
        // Boutons
        bool hasAtLeastOneTarget = config.targetObject1 != null || config.targetObject2 != null;
        EditorGUI.BeginDisabledGroup(!hasAtLeastOneTarget);
        
        if (!EditorApplication.isPlaying)
        {
            if (GUILayout.Button("▶️ LANCER PLAY + LOGGING", GUILayout.Height(40)))
            {
                EditorApplication.isPlaying = true;
            }
        }
        else
        {
            if (!isLogging)
            {
                if (GUILayout.Button("🔴 DÉMARRER LOGGING", GUILayout.Height(40)))
                {
                    StartLogging();
                }
            }
            else
            {
                if (GUILayout.Button("⏹️ ARRÊTER LOGGING", GUILayout.Height(40)))
                {
                    StopLogging();
                }
            }
        }
        
        EditorGUI.EndDisabledGroup();
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📂 Ouvrir dossier logs"))
        {
            string path = Path.Combine(Application.dataPath, "../Logs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            System.Diagnostics.Process.Start(path);
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (config.targetObject1 != null || config.targetObject2 != null)
            {
                EditorApplication.delayCall += () =>
                {
                    if (EditorApplication.isPlaying)
                    {
                        StartLogging();
                    }
                };
            }
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            StopLogging();
        }
    }
    
    static void StartLogging()
    {
        if (config.targetObject1 == null && config.targetObject2 == null)
        {
            Debug.LogError("[Logger] Aucun objet cible sélectionné!");
            return;
        }
        
        // Trouve les objets runtime
        if (config.targetObject1 != null)
        {
            runtimeTarget1 = GameObject.Find(config.targetObject1.name);
            if (runtimeTarget1 == null)
            {
                Debug.LogWarning($"[Logger] Objet 1 '{config.targetObject1.name}' introuvable!");
            }
            else
            {
                Debug.Log($"[Logger] Objet 1 trouvé: {runtimeTarget1.name}");
            }
        }
        
        if (config.targetObject2 != null)
        {
            runtimeTarget2 = GameObject.Find(config.targetObject2.name);
            if (runtimeTarget2 == null)
            {
                Debug.LogWarning($"[Logger] Objet 2 '{config.targetObject2.name}' introuvable!");
            }
            else
            {
                Debug.Log($"[Logger] Objet 2 trouvé: {runtimeTarget2.name}");
            }
        }
        
        if (runtimeTarget1 == null && runtimeTarget2 == null)
        {
            Debug.LogError("[Logger] Aucun des objets n'a été trouvé dans la scène!");
            return;
        }
        
        // Crée le dossier logs
        string logsDir = Path.Combine(Application.dataPath, "../Logs");
        if (!Directory.Exists(logsDir))
        {
            Directory.CreateDirectory(logsDir);
        }
        
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        
        // Crée les fichiers selon le mode
        if (config.useSeparateFiles)
        {
            // MODE: Fichiers séparés
            if (runtimeTarget1 != null)
            {
                string filename1 = $"{runtimeTarget1.name}_log_{timestamp}.txt";
                string filepath1 = Path.Combine(logsDir, filename1);
                logWriter1 = new StreamWriter(filepath1);
                WriteHeader(logWriter1, runtimeTarget1.name);
                
                var logger1 = runtimeTarget1.AddComponent<RuntimeLoggerComponent>();
                logger1.Initialize(logWriter1, config);
                
                Debug.Log($"[Logger] Logging Objet 1 → {filepath1}");
            }
            
            if (runtimeTarget2 != null)
            {
                string filename2 = $"{runtimeTarget2.name}_log_{timestamp}.txt";
                string filepath2 = Path.Combine(logsDir, filename2);
                logWriter2 = new StreamWriter(filepath2);
                WriteHeader(logWriter2, runtimeTarget2.name);
                
                var logger2 = runtimeTarget2.AddComponent<RuntimeLoggerComponent>();
                logger2.Initialize(logWriter2, config);
                
                Debug.Log($"[Logger] Logging Objet 2 → {filepath2}");
            }
        }
        else
        {
            // MODE: Fichier combiné
            string obj1Name = runtimeTarget1 != null ? runtimeTarget1.name : "NULL";
            string obj2Name = runtimeTarget2 != null ? runtimeTarget2.name : "NULL";
            string filename = $"Multi_{obj1Name}_{obj2Name}_{timestamp}.txt";
            string filepath = Path.Combine(logsDir, filename);
            
            logWriterCombined = new StreamWriter(filepath);
            WriteHeaderCombined(logWriterCombined, obj1Name, obj2Name);
            
            if (runtimeTarget1 != null)
            {
                var logger1 = runtimeTarget1.AddComponent<RuntimeLoggerComponent>();
                logger1.Initialize(logWriterCombined, config, 1);
            }
            
            if (runtimeTarget2 != null)
            {
                var logger2 = runtimeTarget2.AddComponent<RuntimeLoggerComponent>();
                logger2.Initialize(logWriterCombined, config, 2);
            }
            
            Debug.Log($"[Logger] Logging combiné → {filepath}");
        }
        
        isLogging = true;
        lastLogTime = Time.time;
        
        EditorApplication.update += UpdateLogging;
    }
    
    static void StopLogging()
    {
        if (!isLogging) return;
        
        isLogging = false;
        EditorApplication.update -= UpdateLogging;
        
        if (logWriterCombined != null)
        {
            logWriterCombined.WriteLine();
            logWriterCombined.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
            logWriterCombined.WriteLine($"FIN DU LOGGING - {System.DateTime.Now}");
            logWriterCombined.Close();
            logWriterCombined = null;
        }
        
        if (logWriter1 != null)
        {
            logWriter1.WriteLine();
            logWriter1.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
            logWriter1.WriteLine($"FIN DU LOGGING - {System.DateTime.Now}");
            logWriter1.Close();
            logWriter1 = null;
        }
        
        if (logWriter2 != null)
        {
            logWriter2.WriteLine();
            logWriter2.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
            logWriter2.WriteLine($"FIN DU LOGGING - {System.DateTime.Now}");
            logWriter2.Close();
            logWriter2 = null;
        }
        
        // Cleanup components
        if (runtimeTarget1 != null)
        {
            var logger = runtimeTarget1.GetComponent<RuntimeLoggerComponent>();
            if (logger != null) Object.DestroyImmediate(logger);
        }
        
        if (runtimeTarget2 != null)
        {
            var logger = runtimeTarget2.GetComponent<RuntimeLoggerComponent>();
            if (logger != null) Object.DestroyImmediate(logger);
        }
        
        Debug.Log("[Logger] Logging arrêté.");
    }
    
    static void WriteHeader(StreamWriter writer, string objectName)
    {
        writer.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
        writer.WriteLine("║                    RUNTIME OBJECT LOGGER                                   ║");
        writer.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        writer.WriteLine();
        writer.WriteLine($"Objet: {objectName}");
        writer.WriteLine($"Début: {System.DateTime.Now}");
        writer.WriteLine($"Intervalle: {config.logInterval}s");
        writer.WriteLine();
        writer.WriteLine("════════════════════════════════════════════════════════════════════════════");
        writer.WriteLine();
        writer.Flush();
    }
    
    static void WriteHeaderCombined(StreamWriter writer, string obj1Name, string obj2Name)
    {
        writer.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
        writer.WriteLine("║               RUNTIME OBJECT LOGGER - MODE COMBINÉ                         ║");
        writer.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        writer.WriteLine();
        writer.WriteLine($"🚁 Objet 1: {obj1Name}");
        writer.WriteLine($"🔫 Objet 2: {obj2Name}");
        writer.WriteLine($"Début: {System.DateTime.Now}");
        writer.WriteLine($"Intervalle: {config.logInterval}s");
        writer.WriteLine();
        writer.WriteLine("════════════════════════════════════════════════════════════════════════════");
        writer.WriteLine();
        writer.Flush();
    }
    
    static void UpdateLogging()
    {
        if (!isLogging || !EditorApplication.isPlaying) return;
        
        if (Time.time - lastLogTime >= config.logInterval)
        {
            lastLogTime = Time.time;
            
            if (config.useSeparateFiles)
            {
                // Mode fichiers séparés
                if (runtimeTarget1 != null && logWriter1 != null)
                {
                    LogObject(runtimeTarget1, logWriter1, cache1, "");
                }
                
                if (runtimeTarget2 != null && logWriter2 != null)
                {
                    LogObject(runtimeTarget2, logWriter2, cache2, "");
                }
            }
            else
            {
                // Mode fichier combiné
                if (logWriterCombined != null)
                {
                    bool hasData = false;
                    System.Text.StringBuilder combinedLog = new System.Text.StringBuilder();
                    
                    combinedLog.AppendLine($"────── [{Time.time:F2}s] ──────");
                    
                    if (runtimeTarget1 != null)
                    {
                        string log1 = LogObjectToString(runtimeTarget1, cache1);
                        if (!string.IsNullOrEmpty(log1))
                        {
                            combinedLog.AppendLine($"🚁 {runtimeTarget1.name}:");
                            combinedLog.Append(log1);
                            hasData = true;
                        }
                    }
                    
                    if (runtimeTarget2 != null)
                    {
                        string log2 = LogObjectToString(runtimeTarget2, cache2);
                        if (!string.IsNullOrEmpty(log2))
                        {
                            if (hasData) combinedLog.AppendLine();
                            combinedLog.AppendLine($"🔫 {runtimeTarget2.name}:");
                            combinedLog.Append(log2);
                            hasData = true;
                        }
                    }
                    
                    if (hasData || !config.logOnlyChanges)
                    {
                        logWriterCombined.Write(combinedLog.ToString());
                        logWriterCombined.WriteLine();
                        logWriterCombined.Flush();
                    }
                }
            }
        }
    }
    
    static void LogObject(GameObject target, StreamWriter writer, ObjectCache cache, string prefix)
    {
        System.Text.StringBuilder log = new System.Text.StringBuilder();
        bool hasChanges = false;
        
        log.AppendLine($"{prefix}────── [{Time.time:F2}s] {target.name} ──────");
        
        // TRANSFORM
        if (config.logTransform)
        {
            Transform t = target.transform;
            
            if (cache.lastPosition == Vector3.zero)
            {
                cache.lastPosition = t.position;
                cache.lastRotation = t.rotation;
                cache.lastScale = t.localScale;
            }
            
            bool transformChanged = false;
            System.Text.StringBuilder transLog = new System.Text.StringBuilder();
            transLog.AppendLine($"{prefix}┌─ TRANSFORM");
            
            if (t.position != cache.lastPosition)
            {
                transformChanged = true;
                hasChanges = true;
                transLog.AppendLine($"{prefix}│  Position: ({t.position.x:F3}, {t.position.y:F3}, {t.position.z:F3}) [CHANGÉ]");
                transLog.AppendLine($"{prefix}│    Delta: ({(t.position - cache.lastPosition).magnitude:F3})");
                cache.lastPosition = t.position;
            }
            else if (!config.logOnlyChanges)
            {
                transLog.AppendLine($"{prefix}│  Position: ({t.position.x:F3}, {t.position.y:F3}, {t.position.z:F3})");
            }
            
            if (t.rotation != cache.lastRotation)
            {
                transformChanged = true;
                hasChanges = true;
                transLog.AppendLine($"{prefix}│  Rotation: ({t.rotation.eulerAngles.x:F1}, {t.rotation.eulerAngles.y:F1}, {t.rotation.eulerAngles.z:F1}) [CHANGÉ]");
                cache.lastRotation = t.rotation;
            }
            
            if (t.localScale != cache.lastScale)
            {
                transformChanged = true;
                hasChanges = true;
                transLog.AppendLine($"{prefix}│  Scale: ({t.localScale.x:F3}, {t.localScale.y:F3}, {t.localScale.z:F3}) [CHANGÉ]");
                cache.lastScale = t.localScale;
            }
            
            transLog.AppendLine($"{prefix}└─");
            
            if (transformChanged || !config.logOnlyChanges)
            {
                log.Append(transLog.ToString());
            }
        }
        
        // ANIMATOR
        if (config.logAnimator)
        {
            Animator animator = target.GetComponent<Animator>();
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                bool animatorChanged = false;
                System.Text.StringBuilder animLog = new System.Text.StringBuilder();
                animLog.AppendLine($"{prefix}┌─ ANIMATOR");
                
                for (int i = 0; i < animator.layerCount; i++)
                {
                    AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(i);
                    
                    if (state.fullPathHash != cache.lastStateHash)
                    {
                        animatorChanged = true;
                        hasChanges = true;
                        
                        string stateName = GetStateName(animator, i, state.fullPathHash);
                        
                        animLog.AppendLine($"{prefix}│  [Layer {i}] TRANSITION → {stateName}");
                        animLog.AppendLine($"{prefix}│    Hash: {state.fullPathHash}");
                        animLog.AppendLine($"{prefix}│    Normalized Time: {state.normalizedTime:F3}");
                        animLog.AppendLine($"{prefix}│    Length: {state.length:F2}s");
                        animLog.AppendLine($"{prefix}│    Speed: {state.speed}");
                        
                        cache.lastStateHash = state.fullPathHash;
                        cache.lastStateName = stateName;
                    }
                    else if (!config.logOnlyChanges)
                    {
                        animLog.AppendLine($"{prefix}│  [Layer {i}] {cache.lastStateName}");
                        animLog.AppendLine($"{prefix}│    Time: {state.normalizedTime:F3}");
                    }
                }
                
                // Paramètres
                foreach (AnimatorControllerParameter param in animator.parameters)
                {
                    float currentValue = 0;
                    string valueStr = "";
                    
                    switch (param.type)
                    {
                        case AnimatorControllerParameterType.Float:
                            currentValue = animator.GetFloat(param.name);
                            valueStr = currentValue.ToString("F3");
                            break;
                        case AnimatorControllerParameterType.Int:
                            currentValue = animator.GetInteger(param.name);
                            valueStr = currentValue.ToString();
                            break;
                        case AnimatorControllerParameterType.Bool:
                            currentValue = animator.GetBool(param.name) ? 1 : 0;
                            valueStr = (currentValue == 1).ToString();
                            break;
                        case AnimatorControllerParameterType.Trigger:
                            currentValue = animator.GetBool(param.name) ? 1 : 0;
                            valueStr = (currentValue == 1) ? "SET" : "UNSET";
                            break;
                    }
                    
                    if (!cache.lastAnimatorParams.ContainsKey(param.name))
                    {
                        cache.lastAnimatorParams[param.name] = currentValue;
                    }
                    
                    if (currentValue != cache.lastAnimatorParams[param.name])
                    {
                        animatorChanged = true;
                        hasChanges = true;
                        animLog.AppendLine($"{prefix}│  Param '{param.name}': {cache.lastAnimatorParams[param.name]} → {valueStr} [CHANGÉ]");
                        cache.lastAnimatorParams[param.name] = currentValue;
                    }
                }
                
                animLog.AppendLine($"{prefix}└─");
                
                if (animatorChanged || !config.logOnlyChanges)
                {
                    log.Append(animLog.ToString());
                }
            }
        }
        
        // COMPONENTS
        if (config.logComponents)
        {
            Component[] components = target.GetComponents<Component>();
            foreach (Component comp in components)
            {
                if (comp == null) continue;
                
                if (comp is Behaviour behaviour)
                {
                    string key = $"{comp.GetType().Name}_enabled";
                    bool wasEnabled = cache.lastValues.ContainsKey(key) && (bool)cache.lastValues[key];
                    
                    if (!cache.lastValues.ContainsKey(key) || wasEnabled != behaviour.enabled)
                    {
                        hasChanges = true;
                        log.AppendLine($"{prefix}• {comp.GetType().Name}: {(behaviour.enabled ? "ENABLED" : "DISABLED")}");
                        cache.lastValues[key] = behaviour.enabled;
                    }
                }
            }
        }
        
        if (hasChanges || !config.logOnlyChanges)
        {
            writer.Write(log.ToString());
            writer.Flush();
        }
    }
    
    static string LogObjectToString(GameObject target, ObjectCache cache)
    {
        System.Text.StringBuilder log = new System.Text.StringBuilder();
        bool hasChanges = false;
        
        // TRANSFORM
        if (config.logTransform)
        {
            Transform t = target.transform;
            
            if (cache.lastPosition == Vector3.zero)
            {
                cache.lastPosition = t.position;
                cache.lastRotation = t.rotation;
                cache.lastScale = t.localScale;
            }
            
            if (t.position != cache.lastPosition)
            {
                hasChanges = true;
                log.AppendLine($"  Position: ({t.position.x:F3}, {t.position.y:F3}, {t.position.z:F3}) Δ{(t.position - cache.lastPosition).magnitude:F3}");
                cache.lastPosition = t.position;
            }
            
            if (t.rotation != cache.lastRotation)
            {
                hasChanges = true;
                log.AppendLine($"  Rotation: ({t.rotation.eulerAngles.x:F1}°, {t.rotation.eulerAngles.y:F1}°, {t.rotation.eulerAngles.z:F1}°)");
                cache.lastRotation = t.rotation;
            }
        }
        
        // ANIMATOR
        if (config.logAnimator)
        {
            Animator animator = target.GetComponent<Animator>();
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
                
                if (state.fullPathHash != cache.lastStateHash)
                {
                    hasChanges = true;
                    string stateName = GetStateName(animator, 0, state.fullPathHash);
                    log.AppendLine($"  État: {stateName} (t={state.normalizedTime:F2})");
                    cache.lastStateHash = state.fullPathHash;
                    cache.lastStateName = stateName;
                }
            }
        }
        
        if (hasChanges || !config.logOnlyChanges)
        {
            return log.ToString();
        }
        
        return "";
    }
    
    static string GetStateName(Animator animator, int layer, int stateHash)
    {
        var controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        if (controller != null && layer < controller.layers.Length)
        {
            var stateMachine = controller.layers[layer].stateMachine;
            foreach (var state in stateMachine.states)
            {
                if (Animator.StringToHash(state.state.name) == stateHash ||
                    Animator.StringToHash($"{stateMachine.name}.{state.state.name}") == stateHash)
                {
                    return state.state.name;
                }
            }
        }
        
        return $"State_{stateHash}";
    }
}

// Component attaché à l'objet pour logger les events
public class RuntimeLoggerComponent : MonoBehaviour
{
    StreamWriter writer;
    ObjectLoggerConfig config;
    int objectID = 0; // 0=combined, 1=obj1, 2=obj2
    string prefix = "";
    
    public void Initialize(StreamWriter w, ObjectLoggerConfig c, int id = 0)
    {
        writer = w;
        config = c;
        objectID = id;
        
        if (id == 1) prefix = "🚁 ";
        else if (id == 2) prefix = "🔫 ";
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (config.logCollisions && writer != null)
        {
            writer.WriteLine($"[{Time.time:F3}s] {prefix}💥 COLLISION ENTER: {collision.gameObject.name}");
            writer.WriteLine($"    Contact points: {collision.contactCount}");
            writer.WriteLine($"    Relative velocity: {collision.relativeVelocity.magnitude:F2}");
            writer.Flush();
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (config.logCollisions && writer != null)
        {
            writer.WriteLine($"[{Time.time:F3}s] {prefix}💨 COLLISION EXIT: {collision.gameObject.name}");
            writer.Flush();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (config.logTriggers && writer != null)
        {
            writer.WriteLine($"[{Time.time:F3}s] {prefix}⚡ TRIGGER ENTER: {other.gameObject.name}");
            writer.Flush();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (config.logTriggers && writer != null)
        {
            writer.WriteLine($"[{Time.time:F3}s] {prefix}⚡ TRIGGER EXIT: {other.gameObject.name}");
            writer.Flush();
        }
    }
    
    void OnEnable()
    {
        if (config.logEnabled && writer != null)
        {
            writer.WriteLine($"[{Time.time:F3}s] {prefix}✅ OBJECT ENABLED");
            writer.Flush();
        }
    }
    
    void OnDisable()
    {
        if (config.logEnabled && writer != null)
        {
            writer.WriteLine($"[{Time.time:F3}s] {prefix}❌ OBJECT DISABLED");
            writer.Flush();
        }
    }
}
#endif
