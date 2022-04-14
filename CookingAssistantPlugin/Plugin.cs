using BepInEx;
using UnityEngine;
using System.Collections;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using HarmonyLib;
using NobleMuffins.MuffinSlicer;
using Logger = BepInEx.Logging.Logger;

namespace CookingAssistantPlugin
{
    class Patch
    {
        [HarmonyPatch(typeof(ProductPart), "KnifeCut")]
        [HarmonyPrefix]
        static void KnifeCutPrefix(ProductPart __instance, out int __state)
        {
            var objid = __instance.gameObject.GetInstanceID();
            __state = objid;
            FileLog.Log("cutprefix_id: " + objid.ToString());
            Console.WriteLine("cutprefix_id: " + objid.ToString());
            var cutcount = __instance.Sliceable.CutCount;
            FileLog.Log(cutcount.ToString());
            Console.WriteLine("pre_cutcount: " + cutcount.ToString());
        }
        
        [HarmonyPatch(typeof(ProductPart), "KnifeCut")]
        [HarmonyPostfix]
        static void KnifeCutPostfix(ref List<ProductPart> __result, int __state) //not sure if this is the right way to use __state
        {
            List<int> childId = new List<int>();
            foreach (ProductPart p in __result)
            {
                var objid = p.gameObject.GetInstanceID();
                childId.Add(objid);
                FileLog.Log(objid.ToString());
                Console.WriteLine("cutpostfix_id: " + objid.ToString());
                var cutcount = p.Sliceable.CutCount;
                FileLog.Log(cutcount.ToString());
                Console.WriteLine("post_outcount: " + cutcount.ToString());
            }
            //have problem
            foreach (Plugin.UnityObjectTree t in Plugin.objectTree)//Go through tree
            {
                Console.WriteLine("treelop");
                if (t.ChildrenIds.Contains(__state))//if ori object is a child
                {
                    Console.WriteLine("cutfunction");
                    t.CutFunction(__state,childId);
                }
            }
        }
        
        
        [HarmonyPatch(typeof(ProductsManager), "CreateProduct", new[] { typeof(Product), typeof(int), typeof(SpawnPoint), typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(bool) })]
        [HarmonyPrefix]
        static void ProductManagerPrefix(Product __instance, int id, object[] __args)
        {
        }
        
        [HarmonyPatch(typeof(ProductsManager), "CreateProduct", new []{typeof(Product), typeof(int), typeof(SpawnPoint), typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(bool)} )]
        [HarmonyPostfix]
        static void ProductManagerPostfix(Product __result, object[] __args, int id)
        {
            var objid = __result.gameObject.GetInstanceID();//this ID or instance.ID
            FileLog.Log(objid.ToString());
            Console.WriteLine("createobject: "+objid.ToString());
            Console.WriteLine("createobject_id: " + id);
        
            Plugin.UnityObjectTree objtree = new Plugin.UnityObjectTree(objid, nameToTypeDictionary[id]);//create object
            Plugin.objectTree.Add(objtree);//push to list
            
        }
        
        

        private static Dictionary<int, string> nameToTypeDictionary = new Dictionary<int, string>
        {
            {0, "tomato"},
            {1, "water"},
            {10, "black_pepper"},
            {107, "champinion"},
            {108, "bulgarian_paprika_red"},
            {109, "bulgarian_paprika_green"},
            {11, "oregano_dried"},
            {111, "bulgarian_paprika_yellow"},
            {112, "jalapeno_paprika"},
            {113, "chili_paprika"},
            {114, "brussels_cabbage"},
            {115, "lime"},
            {116, "gorgonzola_cheese"},
            {117, "pork_carbonate"},
            {118, "clarified_butter"},
            {119, "milk"},
            {12, "honey"},
            {120, "mustard"},
            {121, "avocado_oil"},
            {122, "olive_oil"},
            {123, "rice_vinegar"},
            {124, "balsamic_vinegar"},
            {125, "allspice_powder"},
            {126, "bay_leaf_dried"},
            {127, "cayenne_pepper_powder"},
            {128, "cinnamon_powder"},
            {129, "garlic_dried"},
            {13, "chicken_broth"},
            {130, "horseradish_dried"},
            {131, "nutmeg_powder"},
            {132, "lemon_pepper"},
            {133, "vanilla_powder"},
            {134, "loaf_bread"},
            {135, "basil"},
            {136, "oregano"},
            {137, "mint"},
            {138, "rosemary"},
            {139, "thyme"},
            {14, "peeled_onion"},
            {140, "parsley"},
            {141, "chives"},
            {142, "coriander"},
            {143, "dill"},
            {144, "sage"},
            {145, "bay_leaf"},
            {146, "wine_vinegar"},
            {147, "sausage"},
            {15, "pasta"},
            {16, "garlic"},
            {160, "fried_egg"},
            {161, "egg"},
            {162, "peeled_egg"},
            {165, "t_bone_steak"},
            {166, "red_potato"},
            {168, "starch"},
            {169, "rice_wine"},
            {17, "steak"},
            {170, "sesame_oil"},
            {171, "sichuan_pepper"},
            {172, "peanut_oil"},
            {173, "hosin_sauce"},
            {174, "spring_onion"},
            {177, "chicken_breast"},
            {178, "mooncake"},
            {179, "mooncake"},
            {18, "sour_cream"},
            {180, "mooncake"},
            {181, "mooncake"},
            {182, "star_anis"},
            {183, "cinnamon_stick"},
            {184, "mustard_seeds"},
            {185, "cloves"},
            {187, "helles_marzenbier"},
            {188, "dunkles_marzenbier"},
            {189, "bavarian_beer_vinegar"},
            {190, "elderflower_jelly"},
            {191, "redcurrant_jelly"},
            {192, "dijon_mustard"},
            {193, "dusseldorf_mustard"},
            {194, "parsnip"},
            {195, "red_cabbage"},
            {196, "potato_dumplings"},
            {197, "pork_shoulder"},
            {2, "sunflower_oil"},
            {20, "white_vinegar"},
            {21, "lemon_juice"},
            {23, "jug"},
            {26, "potato"},
            {27, "corn"},
            {28, "radish"},
            {3, "red_wine"},
            {30, "carrot"},
            {31, "apple"},
            {32, "peeled_banana"},
            {33, "peeled_orange"},
            {34, "strawberry"},
            {35, "honey"},
            {36, "sugar"},
            {38, "parsley_root"},
            {39, "mayonnaise"},
            {4, "duck"},
            {42, "penne"},
            {43, "spaghetti"},
            {44, "burger"},
            {45, "burger_bun"},
            {46, "sweet_pepper_powder"},
            {47, "chicken_leg"},
            {48, "shrimp"},
            {49, "broccoli"},
            {5, "raspberry"},
            {52, "dill_dried"},
            {53, "bacon"},
            {54, "lettuce"},
            {56, "white_wine"},
            {57, "white_pepper"},
            {59, "cilantro_leaves_dried"},
            {6, "courgette"},
            {60, "lime_juice"},
            {61, "rosemary_dried"},
            {63, "soy"},
            {64, "mozarella"},
            {65, "meat_bone"},
            {66, "thyme_dried"},
            {68, "coconut_milk"},
            {69, "ginger"},
            {7, "cucumber"},
            {70, "basil_dried"},
            {71, "cumin_powder"},
            {72, "curry_powder"},
            {73, "mint_dried"},
            {74, "herbs_de_provence"},
            {75, "chili_flakes"},
            {76, "lovage_powder"},
            {77, "turmeric_powder"},
            {78, "marjoram"},
            {79, "smoked_pepper"},
            {8, "eggplant"},
            {80, "asparagus"},
            {81, "cheddar"},
            {82, "trout"},
            {83, "salmon_fillet"},
            {84, "lemon"},
            {85, "tuna"},
            {86, "cod"},
            {87, "beetroot"},
            {88, "horseradish"},
            {89, "parmesan"},
            {9, "salt "},
            {90, "pumpkin"},
            {91, "goat_cheese"},
            {92, "moule"},
            {93, "ginger"},
            {94, "pear"},
            {95, "scallops"},
            {97, "tenderloin"},
            {98, "ketchup"},
            {99, "barbecue_sauce"}
        };

    }


    /*
     //class patch
    [HarmonyPatch(typeof(ProductPart), nameof(ProductPart.KnifeCut))]
    class ClassPatch
    {
        static void Prefix(ref List<ProductPart> __result, ProductPart __instance)
        {
            var objid = __instance.gameObject.GetInstanceID();
            FileLog.Log(objid.ToString());
            var cutcount = __instance.Sliceable.CutCount;
            FileLog.Log(cutcount.ToString());
    
        }
        static void Postfix(ref List<ProductPart> __result, ProductPart __instance)
        {
            foreach (ProductPart p in __result)
            {
                var objid = p.gameObject.GetInstanceID();
                FileLog.Log(objid.ToString());
                var cutcount = p.Sliceable.CutCount;
                FileLog.Log(cutcount.ToString());
            }
    
            //Plugin.Instance.AddData();
        }
    }
    */


    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public string host = "localhost";
        public int port = 9999;
        private bool _socketReady = false;
        private TcpClient _tcpSocket;
        private NetworkStream _netStream;
        private StreamWriter _socketWriter;
        private StreamReader _socketReader;
        private Thread _rec;
        private Thread _send;
        private ConcurrentQueue<string> _recQueue;
        static BlockingCollection<string> _sendQueue;
        private bool _running;


        public static Plugin Instance;

        public struct UnityObjectTree
        {
            public int OriginalId;
            public List<int> ChildrenIds;
            public string ObjectType;

            public UnityObjectTree(int originalId, string objectType)
            {
                this.OriginalId = originalId;
                this.ObjectType = objectType;
                this.ChildrenIds = new List<int> {OriginalId};


                Console.WriteLine("type:"+objectType);
                Console.WriteLine("id:" + originalId);
                //send info
                KitchenData _kdata = new KitchenData
                {
                    Sender = "Unity",
                    Action = "Create",
                    Type = ObjectType,
                    Name = OriginalId.ToString(),
                    Status = ""

                };
                var senddata = JsonUtility.ToJson(_kdata);
                _sendQueue.Add(senddata);
            }

            public void CutFunction(int preId, List<int> postIds)
            {
                ChildrenIds.Remove(preId);
                ChildrenIds.AddRange(postIds);
                //send info
                Console.WriteLine("preID:" + preId);
                foreach (var pi in postIds)
                {
                    Console.WriteLine("child_id:" + pi);
                }

                KitchenData _kdata = new KitchenData
                {
                    Sender = "Unity",
                    Action = "Cut",
                    Type = ObjectType,
                    Name = OriginalId.ToString(),
                    Status = ChildrenIds.Count.ToString()
                };

                var senddata = JsonUtility.ToJson(_kdata);
                _sendQueue.Add(senddata);
            }
        };

        public static List<UnityObjectTree> objectTree;

        private class KitchenData
        {
            public string Sender;//Unity
            public string Action;
            public string Type;
            public string Name;
            public string Status;
        }

        private KitchenData _tempobject;
        private void Awake()
        {

            objectTree = new List<UnityObjectTree>();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            
            // Socket Logic
            _recQueue = new ConcurrentQueue<string>();
            _sendQueue = new BlockingCollection<string>();
            _running = true;

            SetupSocket();
            //_rec = new Thread(ReceiveData);
            //_rec.Start();//Start receive data thread
            _send = new Thread(SendDataThread);
            _send.Start();//Start send data thread

            Logger.LogInfo($"Start socket!");

            _tempobject = new KitchenData
            {
                Action = "sending the data",
                //Food = "egg",
                Status = "empty"
            };

            var initial = JsonUtility.ToJson(_tempobject);
            _sendQueue.Add(initial);
            Logger.LogInfo($"send init!");

            Harmony.DEBUG = true;
            Instance = this;
            //Patch
            var ins = new Harmony("tester");
            ins.PatchAll(typeof(Patch));
            Logger.LogInfo($"Patch!");

        }

        public void AddData(string a)
        {
            _sendQueue.Add(a);
        }

        private void Update()
        {
            //Logger.LogInfo($"Updated");
        }

        private void ReceiveData()
        {
            while (_running)//Always ready to receive
            {
                var receivedData = _socketReader.ReadLine();
                Debug.Log("Python controller sent: " + receivedData);
                _recQueue.Enqueue(receivedData);
            }
        }

        private void SendDataThread()
        {
            while (_running)
            {

                string jsonText;
                var data = _sendQueue.Take();
                //var test = JsonUtility.ToJson(data);
                WriteSocket(data + "\n");

            }
        }

        private void OnDestroy()
        {
            _running = false;
            _rec.Abort();//close threads
            _send.Abort();
            CloseSocket();//close socket

        }

        //...setting up the communication
        private void SetupSocket()
        {
            try
            {
                _tcpSocket = new TcpClient(host, port);
                _netStream = _tcpSocket.GetStream();
                _socketWriter = new StreamWriter(_netStream);
                //TODO
                //_socketWriter.Write("Plugin\n");
                _socketReader = new StreamReader(_netStream);
                _socketReady = true;
            }
            catch (Exception e)
            {
                // Something went wrong
                Debug.Log("Socket error: " + e);
            }
        }

        //... writing to a socket...
        private void WriteSocket(string line)
        {
            if (!_socketReady)
                return;

            _socketWriter.Write(line);
            _socketWriter.Flush();
        }

        //... reading from a socket...


        //... closing a socket...
        private void CloseSocket()
        {
            if (!_socketReady)
                return;

            _socketWriter.Close();
            _socketReader.Close();
            _tcpSocket.Close();
            _socketReady = false;
        }
    }
}
