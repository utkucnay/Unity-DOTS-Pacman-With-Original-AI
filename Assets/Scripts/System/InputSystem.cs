using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
[AlwaysSynchronizeSystem]
public partial class InputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<InputTag>()
            .WithNone<DeadTag>()
            .ForEach((ref InputTag inputTag, ref Translation translation, ref MoveData moveData, in DynamicBuffer<DecisionDir> decisionDirData, in DynamicBuffer<DecisionLocData> DecisionLoc) =>
            {
                bool up = Input.GetKeyDown(KeyCode.W);
                bool down = Input.GetKeyDown(KeyCode.S);
                bool left = Input.GetKeyDown(KeyCode.A);
                bool right = Input.GetKeyDown(KeyCode.D);
                bool IsDecisionPoint = false;

                if (up)
                {
                    inputTag.predDir = Dir.up;
                }
                if (down)
                {
                    inputTag.predDir = Dir.down;
                }
                if (left)
                {
                    inputTag.predDir = Dir.left;
                }
                if (right)
                {
                    inputTag.predDir = Dir.right;
                }

                if (inputTag.mainDir == 0)
                {
                    switch (inputTag.predDir)
                    {
                        case Dir.left:
                            moveData.moveDir.x = -2f;
                            inputTag.mainDir = inputTag.predDir;
                            break;
                        case Dir.right:
                            moveData.moveDir.x = 2f;
                            inputTag.mainDir = inputTag.predDir;
                            break;
                        default:
                            break;
                    }
                }
               
                for (int i = 0; i < DecisionLoc.Capacity; i++)
                {
                    if (math.distance(DecisionLoc[i].Loc, translation.Value) <= 0.0001f)
                    {
                        translation.Value = DecisionLoc[i].Loc;
                        if ((decisionDirData[i].dir & Dir.up) == Dir.up && inputTag.predDir == Dir.up)
                        {
                            moveData.moveDir.y = 2f;
                            translation.Value.y += 0.1f;
                            moveData.moveDir.x = 0f;
                            IsDecisionPoint = true;
                            inputTag.mainDir = Dir.up;
                            break;
                        }
                        if ((decisionDirData[i].dir & Dir.down) == Dir.down && inputTag.predDir == Dir.down)
                        {
                            moveData.moveDir.y = -2f;
                            translation.Value.y -= 0.1f;
                            moveData.moveDir.x = 0f;
                            IsDecisionPoint = true;
                            inputTag.mainDir = Dir.down;
                            break;
                        }
                        if ((decisionDirData[i].dir & Dir.left) == Dir.left && inputTag.predDir == Dir.left)
                        {
                            moveData.moveDir.x = -2f;
                            translation.Value.x -= 0.1f;
                            moveData.moveDir.y = 0f;
                            IsDecisionPoint = true;
                            inputTag.mainDir = Dir.left;
                            break;
                        }
                        if ((decisionDirData[i].dir & Dir.right) == Dir.right && inputTag.predDir == Dir.right)
                        {
                            moveData.moveDir.x = 2f;
                            translation.Value.x += 0.1f;
                            moveData.moveDir.y = 0f;
                            IsDecisionPoint = true;
                            inputTag.mainDir = Dir.right;
                            break;
                        }
                        moveData.moveDir.x = 0f;
                        moveData.moveDir.y = 0f;
                        IsDecisionPoint = true;
                        break;

                    }
                }
                if (!IsDecisionPoint)
                {
                    if ((inputTag.mainDir & Dir.left) == Dir.left || (inputTag.mainDir & Dir.right) == Dir.right)
                    {
                        switch (inputTag.predDir)
                        {
                            case Dir.left:
                                moveData.moveDir.x = -2f;
                                inputTag.mainDir = inputTag.predDir;
                                break;
                            case Dir.right:
                                moveData.moveDir.x = 2f;
                                inputTag.mainDir = inputTag.predDir;
                                break;
                            default:
                                break;
                        }
                    }
                    if ((inputTag.mainDir & Dir.up) == Dir.up || (inputTag.mainDir & Dir.down) == Dir.down)
                    {
                        switch (inputTag.predDir)
                        {
                            case Dir.up:
                                moveData.moveDir.y = 2f;
                                inputTag.mainDir = inputTag.predDir;
                                break;
                            case Dir.down:
                                moveData.moveDir.y = -2f;
                                inputTag.mainDir = inputTag.predDir;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }).WithoutBurst().Run();
    }
}
