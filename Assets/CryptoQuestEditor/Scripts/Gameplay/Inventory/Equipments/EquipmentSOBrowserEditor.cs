﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CryptoQuest.Item.Equipment;
using CryptoQuestEditor.Helper;
using IndiGames.Tools.ScriptableObjectBrowser;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace CryptoQuestEditor
{
    public class EquipmentSOBrowserEditor : ScriptableObjectBrowserEditor<EquipmentPrefab>
    {
        private const int ROW_OFFSET = 2;

        private const string DEFAULT_STORAGE_PATH = "Assets/ScriptableObjects/Inventory/Items/Equipments";

        private const int ROW_ID = 0;
        private const int ROW_LOCALIZED_NAME = 1;
        private const int ROW_EQUIPMENT_TYPE_ID = 9;
        private const int ROW_CHARACTER_LEVEL_REQUIREMENT = 20;
        private const int ROW_EQUIPMENT_SLOT_ID = 11;

        private const int ROW_EQUIPMENT_PART_ID = 8;
        private const int ROW_EQUIPMENT_SPRITE_ID = 12;
        private readonly string[] _tableNames = { "Equipment", "Weapon", "Armor", "Accessory" };
        private Dictionary<int, EquipmentTypeSO> _equipmentTypeMap = new Dictionary<int, EquipmentTypeSO>();
        private const string SPRITE_PATH = "Assets/Arts/";
        private const string TXT_PATH_STORAGE = "Assets/";
        private StreamWriter _writer;

        public EquipmentSOBrowserEditor()
        {
            CreateDataFolder = false;

            DefaultStoragePath = "Assets/ScriptableObjects/Inventory/Items/Equipments";
        }

        public override void ImportBatchData(string directory, Action<ScriptableObject> callback)
        {
            string[] rows = File.ReadAllLines(directory);
            LoadEquipmenTypeMap();
            // string[] firstRow = rows[ROW_OFFSET].Split('\t');
            // string typeIndex = firstRow[0][0].ToString();
            // var txtPath = TXT_PATH_STORAGE + "missing_images_" + GetImageFolder(typeIndex) + ".txt";
            // _writer = new StreamWriter(txtPath, true);
            for (int index = ROW_OFFSET; index < rows.Length; index++)
            {
                // get data form tsv file
                string[] cols = rows[index].Split('\t');

                Debug.Log($"Importing {cols}");

                string id = cols[ROW_ID];

                string currentID = String.Empty;
                if (currentID == id) continue;
                currentID = id;

                var slot = GetEquipmentSlot(ParseData(cols[ROW_EQUIPMENT_SLOT_ID]));

                string name = GetName(cols);
                string path = DefaultStoragePath + "/" + name.Split("_")[0] + "/" + name + ".asset";

                EquipmentPrefab instance = null;

                // find instance if null create new
                instance = (EquipmentPrefab)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentPrefab));
                if (instance == null || !AssetDatabase.Contains(instance))
                {
                    instance = ScriptableObject.CreateInstance<EquipmentPrefab>();
                }

                // import Data
                instance.Editor_SetID(id);
                Debug.Log(id);
                instance.name = name;
                var serializedObject = new SerializedObject(instance);
                SetAllowedSlots(GetAllowedSlots(cols), serializedObject, cols);
                SetRequiredSlots(GetRequiredSlots(cols), serializedObject, cols);
                SetEquipmentType(GetEquipmentType(ParseData(cols[ROW_EQUIPMENT_TYPE_ID])), serializedObject, cols);
                var displayName = LocalizedString(cols, out var description);
                string imageFolder = GetImageFolder(cols[ROW_ID][0].ToString());
                var imageAssetRef = GetSpriteAssetRef(cols[ROW_EQUIPMENT_SPRITE_ID], imageFolder);
                Debug.Log(imageAssetRef);
                SetSprite(imageAssetRef, serializedObject);

                instance.Editor_SetDisplayName(displayName);
                instance.Editor_SetDescription(description);
                // Save data
                if (!AssetDatabase.Contains(instance))
                {
                    AssetDatabase.CreateAsset(instance, path);
                    AssetDatabase.SaveAssets();
                    callback(instance);
                }
                else
                {
                    EditorUtility.SetDirty(instance);
                }
            }

            // _writer.Close();
        }

        private void SetAllowedSlots(ESlot[] types, SerializedObject serializedObject, string[] datas)
        {
            var property = serializedObject.FindProperty("<AllowedSlots>k__BackingField");
            property.ClearArray();
            string[] partIds = datas[ROW_EQUIPMENT_PART_ID].Split(',');
            if (partIds.Length < 2)
                for (var index = 0; index < types.Length; index++)
                {
                    property.InsertArrayElementAtIndex(index);
                    property.GetArrayElementAtIndex(index).boxedValue = types[index];
                }
            else
            {
                property.InsertArrayElementAtIndex(0);
                property.GetArrayElementAtIndex(0).boxedValue = types[0];
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void SetEquipmentType(EquipmentTypeSO equipmentTypeSo, SerializedObject serializedObject,
            string[] datas)
        {
            var property = serializedObject.FindProperty("<EquipmentType>k__BackingField");
            property.objectReferenceValue = equipmentTypeSo;
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void SetSprite(AssetReferenceT<Sprite> image, SerializedObject serializedObject)
        {
            var property = serializedObject.FindProperty("<Image>k__BackingField");
            property.boxedValue = image;
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private AssetReferenceT<Sprite> GetSpriteAssetRef(string spriteName, string folderName)
        {
            var path = SPRITE_PATH + folderName + "/" + spriteName + ".png";
            var guid = AssetDatabase.AssetPathToGUID(path);
            // if (string.IsNullOrEmpty(guid))
            //     _writer.WriteLine(spriteName);

            Debug.Log("Sprite: getting" + guid);
            return new AssetReferenceT<Sprite>(guid);
        }

        private void SetRequiredSlots(ESlot[] types, SerializedObject serializedObject, string[] datas)
        {
            var property = serializedObject.FindProperty("<RequiredSlots>k__BackingField");
            property.ClearArray();
            string[] partIds = datas[ROW_EQUIPMENT_PART_ID].Replace(" ", "").Split(',');
            if (!partIds.Contains("7"))
                for (var index = 0; index < types.Length; index++)
                {
                    property.InsertArrayElementAtIndex(index);
                    property.GetArrayElementAtIndex(index).boxedValue = types[index];
                }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }


        private LocalizedString LocalizedString(string[] rows, out LocalizedString description)
        {
            LocalizedString displayName = new LocalizedString();
            description = new LocalizedString();

            StringTableCollection table = null;
            SharedTableData.SharedTableEntry tableEntry = null;

            foreach (var tableName in _tableNames)
            {
                table = LocalizationEditorSettings.GetStringTableCollection(tableName);
                if (table == null) continue;

                tableEntry = table.SharedData.GetEntryFromReference(rows[ROW_LOCALIZED_NAME]);

                if (tableEntry != null) break;
            }

            if (tableEntry == null) return displayName;
            displayName.TableReference = table.TableCollectionNameReference;
            description.TableReference = table.TableCollectionNameReference;
            displayName.TableEntryReference = tableEntry.Id;
            description.TableEntryReference = tableEntry.Id;

            return displayName;
        }

        private ESlot[] GetAllowedSlots(string[] datas)
        {
            string data = datas[ROW_EQUIPMENT_PART_ID];
            string[] rows = data.Split(',');
            ESlot[] slots = new ESlot[rows.Length];
            if (data == "7")
            {
                int newSize = 2;
                Array.Resize(ref slots, newSize);
                slots[0] = ESlot.Accessory1;
                slots[1] = ESlot.Accessory2;
                return slots;
            }

            for (int i = 0; i < rows.Length; i++)
            {
                slots[i] = GetEquipmentSlot(ParseData(rows[i]));
            }

            return slots;
        }

        private ESlot[] GetRequiredSlots(string[] datas)
        {
            string data = datas[ROW_EQUIPMENT_PART_ID];
            string[] rows = data.Split(',');
            ESlot[] slots = new ESlot[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                slots[i] = GetEquipmentSlot(ParseData(rows[i]));
            }

            return slots;
        }

        private EquipmentTypeSO GetEquipmentType(int id)
        {
            return _equipmentTypeMap[id];
        }

        private int ParseData(string data) => string.IsNullOrEmpty(data) ? 0 : int.Parse(data);
        private ESlot GetEquipmentSlot(int value) => (ESlot)value;

        private string GetName(string[] data)
        {
            var normalizedName = data[5].Split(" ")[0] + "_" + data[0];
            if (data[0][0].ToString() == "1")
                return "Weapon" + "_" + data[0];
            return normalizedName;
        }

        private void LoadEquipmenTypeMap()
        {
            var guids = AssetDatabase.FindAssets("t:EquipmentTypeSO");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<EquipmentTypeSO>(path);
                _equipmentTypeMap.TryAdd(asset.Id, asset);
            }
        }

        private string GetImageFolder(string id)
        {
            int typeId = int.Parse(id);
            var typeEnum = (EImageFolder)typeId;

            return typeEnum.ToString();
        }

        private enum EImageFolder
        {
            Weapons = 1,
            DefensiveEQs = 2,
            Accessories = 3,
        }
    }
}