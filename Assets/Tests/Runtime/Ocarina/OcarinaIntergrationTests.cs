using System.Collections;
using System.Collections.Generic;
using CryptoQuest.Character.MonoBehaviours;
using CryptoQuest.Events;
using CryptoQuest.Map;
using IndiGames.Core.Events.ScriptableObjects;
using IndiGames.Core.SceneManagementSystem.Events.ScriptableObjects;
using IndiGames.Core.SceneManagementSystem.ScriptableObjects;
using NUnit.Framework;
using Tests.Runtime.Ocarina;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CryptoQuest.Tests.Runtime.OcarinaIntergrationTests
{
    public class OcarinaIntergrationTests
    {
        private readonly WaitForSeconds SECONDS_TO_WAIT = new WaitForSeconds(6);
        private LoadSceneEventChannelSO _loadMapEvent;
        private string STARTUP_SCENE_NAME = "Startup";
        private PathStorageSO _pathStorage;
        private MapPathEventChannelSO _destinationSelectedEvent;
        private VoidEventChannelSO _destinationConfirmEvent;


        [UnityTest]
        public IEnumerator OcarinaBehaviour_SetupCorrectly()
        {
            yield return SceneManager.LoadSceneAsync(STARTUP_SCENE_NAME, LoadSceneMode.Single);

            Assert.That(SceneManager.GetActiveScene().name == STARTUP_SCENE_NAME);

            yield return SECONDS_TO_WAIT;
            Assert.That(SceneManager.GetSceneByName("GlobalManagers").isLoaded);
            OcarinaBehaviour ocarinaBehaviour = GameObject.FindObjectOfType<OcarinaBehaviour>();
            Assert.NotNull(ocarinaBehaviour);

            GetNecessaryScriptableObjects();
            Assert.NotNull(_pathStorage);
            Assert.NotNull(_destinationSelectedEvent);
            Assert.NotNull(_destinationConfirmEvent);

            MapPathSO newPath = ScriptableObject.CreateInstance<MapPathSO>();
            _destinationSelectedEvent.RaiseEvent(newPath);
            _destinationConfirmEvent.RaiseEvent();
            yield return SECONDS_TO_WAIT;
            Assert.That(SceneManager.GetSceneByName("WorldMap").isLoaded);
        }

        private void GetNecessaryScriptableObjects()
        {
            string[] pathStorageGuids = AssetDatabase.FindAssets("t: PathStorageSO");
            string pathStorageSOpath = UnityEditor.AssetDatabase.GUIDToAssetPath(pathStorageGuids[0]);
            _pathStorage = UnityEditor.AssetDatabase.LoadAssetAtPath<PathStorageSO>(pathStorageSOpath);

            string[] destinationSelectedEventGuids = AssetDatabase.FindAssets("t: MapPathEventChannelSO");
            string destinationSelectedEventSOpath =
                UnityEditor.AssetDatabase.GUIDToAssetPath(destinationSelectedEventGuids[0]);
            _destinationSelectedEvent =
                UnityEditor.AssetDatabase.LoadAssetAtPath<MapPathEventChannelSO>(destinationSelectedEventSOpath);

            string[] destinationConfirmEventGuids = AssetDatabase.FindAssets("t: VoidEventChannelSO");
            foreach (string guid in destinationConfirmEventGuids)
            {
                string destinationConfirmEventSOpath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                VoidEventChannelSO eventSo =
                    UnityEditor.AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>(destinationConfirmEventSOpath);
                if (eventSo.name == "DestinationConfirmEvent")
                {
                    _destinationConfirmEvent = eventSo;
                    break;
                }
            }
        }


        [UnityTest]
        public IEnumerator UseOcarina_WithOut_Selected_Destination_Still_Load_WorldMap()
        {
            yield return SceneManager.LoadSceneAsync(STARTUP_SCENE_NAME, LoadSceneMode.Single);

            Assert.That(SceneManager.GetActiveScene().name == STARTUP_SCENE_NAME);

            yield return SECONDS_TO_WAIT;
            Assert.That(SceneManager.GetSceneByName("GlobalManagers").isLoaded);
            OcarinaBehaviour ocarinaBehaviour = GameObject.FindObjectOfType<OcarinaBehaviour>();
            Assert.NotNull(ocarinaBehaviour);

            if (_destinationConfirmEvent == null || _destinationSelectedEvent == null || _pathStorage == null)
                GetNecessaryScriptableObjects();

            _destinationConfirmEvent.RaiseEvent();
            yield return SECONDS_TO_WAIT;
            bool isWorldMapLoaded = SceneManager.GetSceneByName("WorldMap").isLoaded;
            Assert.IsTrue(isWorldMapLoaded);
        }

        [UnityTest]
        public IEnumerator UseOcarina_WithOut_Selected_Destination_Place_Player_At_Default_Pos()
        {
            yield return SceneManager.LoadSceneAsync(STARTUP_SCENE_NAME, LoadSceneMode.Single);

            Assert.That(SceneManager.GetActiveScene().name == STARTUP_SCENE_NAME);

            yield return SECONDS_TO_WAIT;
            Assert.That(SceneManager.GetSceneByName("GlobalManagers").isLoaded);
            OcarinaBehaviour ocarinaBehaviour = GameObject.FindObjectOfType<OcarinaBehaviour>();
            Assert.NotNull(ocarinaBehaviour);

            if (_destinationConfirmEvent == null || _destinationSelectedEvent == null || _pathStorage == null)
                GetNecessaryScriptableObjects();
            _destinationConfirmEvent.RaiseEvent();
            yield return SECONDS_TO_WAIT;
            Assert.That(SceneManager.GetSceneByName("WorldMap").isLoaded);
            Vector2 defaultSpawnPos = GameObject.Find("DefaultSpawnLocation").transform.position;
            Vector2 heroGoPos = GameObject.FindObjectOfType<HeroBehaviour>().gameObject.transform.position;
            Assert.That(heroGoPos == defaultSpawnPos);
        }

        [UnityTest]
        public IEnumerator SelectDestination_NotUpdatePath_Until_UseOcarina()
        {
            yield return SceneManager.LoadSceneAsync(STARTUP_SCENE_NAME, LoadSceneMode.Single);

            Assert.That(SceneManager.GetActiveScene().name == STARTUP_SCENE_NAME);

            yield return SECONDS_TO_WAIT;
            Assert.That(SceneManager.GetSceneByName("GlobalManagers").isLoaded);
            OcarinaBehaviour ocarinaBehaviour = GameObject.FindObjectOfType<OcarinaBehaviour>();
            Assert.NotNull(ocarinaBehaviour);

            if (_destinationConfirmEvent == null || _destinationSelectedEvent == null || _pathStorage == null)
                GetNecessaryScriptableObjects();
            MapPathSO newPath = ScriptableObject.CreateInstance<MapPathSO>();
            newPath.name = "newPath";
            _destinationSelectedEvent.RaiseEvent(newPath);
            Assert.That(_pathStorage.LastTakenPath != newPath);

            _destinationConfirmEvent.RaiseEvent();
            Assert.That(_pathStorage.LastTakenPath == newPath);
        }

        [UnityTest]
        public IEnumerator UseOcarina_To_PortCity_PlacePlayer_At_Entrance()
        {
            yield return SceneManager.LoadSceneAsync(STARTUP_SCENE_NAME, LoadSceneMode.Single);

            Assert.That(SceneManager.GetActiveScene().name == STARTUP_SCENE_NAME);

            yield return SECONDS_TO_WAIT;
            Assert.That(SceneManager.GetSceneByName("GlobalManagers").isLoaded);
            OcarinaBehaviour ocarinaBehaviour = GameObject.FindObjectOfType<OcarinaBehaviour>();
            Assert.NotNull(ocarinaBehaviour);

            if (_destinationConfirmEvent == null || _destinationSelectedEvent == null || _pathStorage == null)
                GetNecessaryScriptableObjects();
            string[] guids = AssetDatabase.FindAssets("t:MapPathSO");
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                MapPathSO mapPathSo =
                    UnityEditor.AssetDatabase.LoadAssetAtPath<MapPathSO>(path);
                if (mapPathSo.name == "Ocarina_WorldMap.PortCity")
                {
                    _destinationSelectedEvent.RaiseEvent(mapPathSo);
                    _destinationConfirmEvent.RaiseEvent();
                    break;
                }
            }

            yield return SECONDS_TO_WAIT;
            GoFrom[] goFromGOs = GameObject.FindObjectsOfType<GoFrom>();
            Vector2 portCityEntrancePos = new Vector2(0, 0);
            foreach (GoFrom goFromGO in goFromGOs)
            {
                GoFrom goFrom = goFromGO.GetComponent<GoFrom>();
                if (goFrom.MapPath.name == "Ocarina_WorldMap.PortCity")
                {
                    portCityEntrancePos = goFromGO.transform.position;
                    break;
                }
            }

            GameObject heroGo = GameObject.FindObjectOfType<HeroBehaviour>().gameObject;
            Assert.NotNull(heroGo);
            Assert.That((Vector2)heroGo.transform.position == (Vector2)portCityEntrancePos);
        }
    }
}