﻿using System.Collections;
using CryptoQuest.EditorTool;
using CryptoQuest.Quest.Actions;
using CryptoQuest.Quest.Components;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CryptoQuest.Quest.Actor.Categories
{
    [CreateAssetMenu(menuName = "QuestSystem/Actor/ExecuteActionActorSO",
        fileName = "ExecuteActionActor")]
    public class ExecuteActionActorSo : ActorSO<ExecuteActionActorInfo>
    {
        [field: Header("Config Settings")]
        [field: SerializeField]
        public NextAction Action { get; private set; }

        [field: SerializeField] public ECollideActionType CollideActionType { get; private set; }
        [field: SerializeField] public bool IsRepeatable { get; private set; } = false;

        public override ActorInfo CreateActor() =>
            new ExecuteActionActorInfo(this);
    }

    public class ExecuteActionActorInfo : ActorInfo<ExecuteActionActorSo>
    {
        public ExecuteActionActorInfo(ExecuteActionActorSo actorSo) : base(actorSo) { }

        public ExecuteActionActorInfo() { }

        public override IEnumerator Spawn(Transform parent)
        {
            AsyncOperationHandle<GameObject> handle =
                Data.Prefab.InstantiateAsync(parent.position, parent.rotation, parent);

            yield return handle;

            TriggerActionCollider actor = handle.Result.GetComponent<TriggerActionCollider>();
            actor.SetAction(Data.Action);
            actor.SetCollideActionType(Data.CollideActionType);
            actor.SetRepeatType(Data.IsRepeatable);

            if (!parent.TryGetComponent<ShowCubeWireUtil>(out var showCubeWireUtil)) yield break;
            actor.SetBoxSize(showCubeWireUtil.SizeBox);

            if (!handle.Result.TryGetComponent<BoxCollider2D>(out var targetCollider2D)) yield break;
            if (!parent.TryGetComponent<BoxCollider2D>(out var parentCollider2D)) yield break;

            targetCollider2D.size = parentCollider2D.size;
        }
    }
}