using CoxusHouse.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoxusHouse.EditorTools
{
    // Builds the M0 placeholder house from primitives into the open scene.
    // Running it again replaces the previous "Graybox" root, so edit the layout below and re-run.
    // Replace this layout with the real floor plan once it is sketched.
    public static class GrayboxBuilder
    {
        private const string RootName = "Graybox";
        private const string MaterialFolder = "Assets/_Project/Materials/Graybox";
        private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player.prefab";

        private const float WallHeight = 2.6f;
        private const float WallThickness = 0.2f;
        private const float DoorWidth = 1.0f;
        private const float DoorHeight = 2.1f;

        // Floor areas in metres, top view: x to the right, y on the rect is world z.
        private static readonly (string name, Rect area, bool indoor)[] Rooms =
        {
            ("LivingRoom", new Rect(-6f, 0f, 6f, 6f), true),
            ("Hallway", new Rect(0f, 0f, 2f, 10f), true),
            ("UtilityRoom", new Rect(2f, 6f, 3f, 4f), true),
            ("Porch", new Rect(-1f, -4f, 4f, 4f), false),
        };

        // Each wall runs along x or z. Doors are distances from the wall's start to each opening's centre.
        private static readonly (string name, Vector2 from, Vector2 to, float[] doors)[] Walls =
        {
            ("Living_South", new Vector2(-6f, 0f), new Vector2(0f, 0f), new float[0]),
            ("Living_West", new Vector2(-6f, 0f), new Vector2(-6f, 6f), new float[0]),
            ("Living_North", new Vector2(-6f, 6f), new Vector2(0f, 6f), new float[0]),
            ("Living_Hallway", new Vector2(0f, 0f), new Vector2(0f, 6f), new[] { 3f }),
            ("Hallway_West", new Vector2(0f, 6f), new Vector2(0f, 10f), new float[0]),
            ("Hallway_East", new Vector2(2f, 0f), new Vector2(2f, 6f), new float[0]),
            ("Hallway_Utility", new Vector2(2f, 6f), new Vector2(2f, 10f), new[] { 2f }),
            ("FrontWall", new Vector2(0f, 0f), new Vector2(2f, 0f), new[] { 1f }),
            ("Hallway_North", new Vector2(0f, 10f), new Vector2(2f, 10f), new float[0]),
            ("Utility_South", new Vector2(2f, 6f), new Vector2(5f, 6f), new float[0]),
            ("Utility_East", new Vector2(5f, 6f), new Vector2(5f, 10f), new float[0]),
            ("Utility_North", new Vector2(2f, 10f), new Vector2(5f, 10f), new float[0]),
        };

        private static readonly Vector3[] SpawnPoints =
        {
            new Vector3(-4.5f, 0f, 1.5f),
            new Vector3(-1.5f, 0f, 1.5f),
            new Vector3(-4.5f, 0f, 4.5f),
            new Vector3(-1.5f, 0f, 4.5f),
        };

        [MenuItem("Coxu/Build Graybox House")]
        public static void Build()
        {
            GameObject existing = GameObject.Find(RootName);
            if (existing != null)
            {
                Undo.DestroyObjectImmediate(existing);
            }

            var root = new GameObject(RootName);
            Undo.RegisterCreatedObjectUndo(root, "Build Graybox House");

            Material floorMat = GetOrCreateMaterial("Floor", new Color(0.45f, 0.45f, 0.48f));
            Material wallMat = GetOrCreateMaterial("Wall", new Color(0.82f, 0.8f, 0.74f));
            Material propMat = GetOrCreateMaterial("Prop", new Color(0.55f, 0.38f, 0.25f));
            Material pizzaMat = GetOrCreateMaterial("Pizza", new Color(0.55f, 0.95f, 0.2f));
            Material markerMat = GetOrCreateMaterial("Marker", new Color(0.2f, 0.5f, 0.95f));

            Transform rooms = Group("Rooms", root.transform);
            Transform lights = Group("Lights_Normal", root.transform);
            foreach (var room in Rooms)
            {
                BuildRoom(room.name, room.area, room.indoor, rooms, lights, floorMat, wallMat);
            }

            Transform walls = Group("Walls", root.transform);
            foreach (var wall in Walls)
            {
                BuildWall(wall.name, wall.from, wall.to, wall.doors, walls, wallMat);
            }

            Transform props = Group("Props", root.transform);
            Box("PizzaTable", new Vector3(-3f, 0.375f, 3f), new Vector3(1.4f, 0.75f, 0.9f), props, propMat);
            GameObject pizza = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Place(pizza, "Pizza", new Vector3(-3f, 0.77f, 3f), new Vector3(0.45f, 0.02f, 0.45f), props, pizzaMat);
            Box("BreakerBox", new Vector3(4.85f, 1.4f, 8f), new Vector3(0.1f, 0.6f, 0.5f), props, markerMat);

            Transform markers = Group("Markers", root.transform);
            Marker("FuseSpawn_A", new Vector3(-5.5f, 0.5f, 5.5f), markers);
            Marker("FuseSpawn_B", new Vector3(1f, 0.5f, 9.5f), markers);
            Marker("ExitZone", new Vector3(1f, 0f, -3f), markers);
            Transform spawns = Group("SpawnPoints", root.transform);
            for (int i = 0; i < SpawnPoints.Length; i++)
            {
                Marker($"SpawnPoint_{i + 1}", SpawnPoints[i], spawns);
            }

            PlacePlayer(SpawnPoints[0]);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Graybox house built. Save the scene to keep it.");
        }

        private static void BuildRoom(string name, Rect area, bool indoor, Transform rooms, Transform lights, Material floorMat, Material wallMat)
        {
            Transform room = Group(name, rooms);
            Vector3 centre = new Vector3(area.center.x, 0f, area.center.y);
            Box("Floor", centre + Vector3.down * 0.05f, new Vector3(area.width, 0.1f, area.height), room, floorMat);
            if (!indoor)
            {
                return;
            }

            Box("Ceiling", centre + Vector3.up * (WallHeight + 0.05f), new Vector3(area.width, 0.1f, area.height), room, wallMat);

            var lightObject = new GameObject($"Light_{name}");
            lightObject.transform.SetParent(lights, false);
            lightObject.transform.position = centre + Vector3.up * (WallHeight - 0.3f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = Mathf.Max(area.width, area.height) * 1.2f;
            light.intensity = 2f;
            light.color = new Color(1f, 0.92f, 0.8f);
        }

        // Splits a wall into solid pieces around each door opening, with a lintel above each opening.
        private static void BuildWall(string name, Vector2 from, Vector2 to, float[] doors, Transform parent, Material material)
        {
            Transform wall = Group(name, parent);
            float length = Vector2.Distance(from, to);
            Vector2 direction = (to - from).normalized;
            float halfDoor = DoorWidth * 0.5f;
            float halfThickness = WallThickness * 0.5f;

            float start = -halfThickness; // overlap at both ends so corners close
            int piece = 0;
            foreach (float door in doors)
            {
                WallPiece($"Piece_{piece++}", from, direction, start, door - halfDoor, 0f, WallHeight, wall, material);
                WallPiece($"Lintel_{piece++}", from, direction, door - halfDoor, door + halfDoor, DoorHeight, WallHeight, wall, material);
                start = door + halfDoor;
            }
            WallPiece($"Piece_{piece}", from, direction, start, length + halfThickness, 0f, WallHeight, wall, material);
        }

        private static void WallPiece(string name, Vector2 from, Vector2 direction, float startAlong, float endAlong,
            float bottom, float top, Transform parent, Material material)
        {
            float pieceLength = endAlong - startAlong;
            if (pieceLength <= 0.01f)
            {
                return;
            }

            Vector2 mid = from + direction * ((startAlong + endAlong) * 0.5f);
            bool alongX = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
            Vector3 size = alongX
                ? new Vector3(pieceLength, top - bottom, WallThickness)
                : new Vector3(WallThickness, top - bottom, pieceLength);
            Box(name, new Vector3(mid.x, (bottom + top) * 0.5f, mid.y), size, parent, material);
        }

        private static void PlacePlayer(Vector3 position)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (prefab == null)
            {
                prefab = CreatePlayerPrefab();
            }

            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            if (player == null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                Undo.RegisterCreatedObjectUndo(instance, "Place Player");
                player = instance.GetComponent<PlayerController>();
            }
            Undo.RecordObject(player.transform, "Place Player");
            player.transform.SetPositionAndRotation(position, Quaternion.identity);

            // The player brings its own camera, so switch off the scene's default one.
            foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            {
                if (camera.GetComponentInParent<PlayerController>() == null)
                {
                    Undo.RecordObject(camera.gameObject, "Disable Scene Camera");
                    camera.gameObject.SetActive(false);
                }
            }
        }

        private static GameObject CreatePlayerPrefab()
        {
            var player = new GameObject("Player");
            var characterController = player.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.35f;
            characterController.center = new Vector3(0f, 0.9f, 0f);
            characterController.stepOffset = 0.35f;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(player.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            body.transform.localScale = new Vector3(0.7f, 0.9f, 0.7f);
            Object.DestroyImmediate(body.GetComponent<Collider>());
            body.GetComponent<Renderer>().sharedMaterial = GetOrCreateMaterial("Player", new Color(0.95f, 0.55f, 0.2f));

            var pivot = new GameObject("CameraPivot");
            pivot.transform.SetParent(player.transform, false);
            pivot.transform.localPosition = new Vector3(0f, 1.65f, 0f);

            var cameraObject = new GameObject("Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(pivot.transform, false);
            var camera = cameraObject.AddComponent<Camera>();
            camera.nearClipPlane = 0.05f;
            camera.fieldOfView = 75f;
            cameraObject.AddComponent<AudioListener>();

            var controller = player.AddComponent<PlayerController>();
            var serialized = new SerializedObject(controller);
            serialized.FindProperty("_cameraPivot").objectReferenceValue = pivot.transform;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EnsureFolder("Assets/_Project/Prefabs");
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(player, PlayerPrefabPath);
            Object.DestroyImmediate(player);
            return prefab;
        }

        private static Transform Group(string name, Transform parent)
        {
            var group = new GameObject(name);
            group.transform.SetParent(parent, false);
            return group.transform;
        }

        private static void Box(string name, Vector3 position, Vector3 size, Transform parent, Material material)
        {
            Place(GameObject.CreatePrimitive(PrimitiveType.Cube), name, position, size, parent, material);
        }

        private static void Place(GameObject primitive, string name, Vector3 position, Vector3 scale, Transform parent, Material material)
        {
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            primitive.transform.position = position;
            primitive.transform.localScale = scale;
            primitive.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void Marker(string name, Vector3 position, Transform parent)
        {
            var marker = new GameObject(name);
            marker.transform.SetParent(parent, false);
            marker.transform.position = position;
        }

        private static Material GetOrCreateMaterial(string name, Color color)
        {
            string path = $"{MaterialFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
            {
                return material;
            }

            EnsureFolder(MaterialFolder);
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }
            int slash = path.LastIndexOf('/');
            string parent = path.Substring(0, slash);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, path.Substring(slash + 1));
        }
    }
}
