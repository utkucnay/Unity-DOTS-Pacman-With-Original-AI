using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

public partial class AnimatorSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        Entities.WithAll<AnimatorData>()
            .ForEach((in AnimatorData animData, in SpriteRenderer spriteRenderer) => {
                var animatorRef = spriteRenderer.gameObject.AddComponent<Animator>();
                animatorRef.runtimeAnimatorController = animData.animatorController;
                animData.animator = animatorRef;
            }).WithoutBurst().Run();
        
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<AnimatorData>()
            .ForEach((in AnimatorData animData,in MoveData moveData) => {
                animData.animator.SetFloat("DirX",moveData.moveDir.x);
                animData.animator.SetFloat("DirY", moveData.moveDir.y);
            }).WithoutBurst().Run();
    }
}

