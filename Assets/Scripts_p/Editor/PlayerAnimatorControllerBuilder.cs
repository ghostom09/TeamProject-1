using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class PlayerAnimatorControllerBuilder
{
    private const string Idle = "Idle";
    private const string Run = "Run";
    private const string Attack = "Attack";
    private const string Die = "Die";
    private const string JumpReady = "JumpReady";
    private const string JumpUp = "JumpUp";
    private const string JumpDown = "JumpDown";

    [MenuItem("Tools/Player/Create Animator Controller From Selected Character")]
    public static void CreateControllerFromSelectedCharacter()
    {
        CharacterData character = Selection.activeObject as CharacterData;

        if (character == null)
        {
            EditorUtility.DisplayDialog("Player Animator", "CharacterData asset을 선택한 뒤 다시 실행하세요.", "OK");
            return;
        }

        string characterPath = AssetDatabase.GetAssetPath(character);
        string directory = System.IO.Path.GetDirectoryName(characterPath)?.Replace("\\", "/");
        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/{character.name}Player.controller");

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(assetPath);
        AddParameters(controller);
        BuildStateMachine(controller);

        character.AnimatorController = controller;
        EditorUtility.SetDirty(character);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = controller;
        EditorUtility.DisplayDialog("Player Animator", $"{character.name} Animator Controller 생성 완료.", "OK");
    }

    private static void AddParameters(AnimatorController controller)
    {
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("MoveX", AnimatorControllerParameterType.Float);
        controller.AddParameter("YVelocity", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
    }

    private static void BuildStateMachine(AnimatorController controller)
    {
        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;

        AnimatorState idle = AddState(controller, stateMachine, Idle, new Vector3(250, 80, 0));
        AnimatorState run = AddState(controller, stateMachine, Run, new Vector3(520, 80, 0));
        AnimatorState jumpReady = AddState(controller, stateMachine, JumpReady, new Vector3(250, 260, 0));
        AnimatorState jumpUp = AddState(controller, stateMachine, JumpUp, new Vector3(520, 260, 0));
        AnimatorState jumpDown = AddState(controller, stateMachine, JumpDown, new Vector3(790, 260, 0));
        AnimatorState attack = AddState(controller, stateMachine, Attack, new Vector3(520, -120, 0));
        AnimatorState die = AddState(controller, stateMachine, Die, new Vector3(790, -120, 0));

        stateMachine.defaultState = idle;

        AddConditionTransition(idle, run, false, "IsMoving", AnimatorConditionMode.If);
        AddConditionTransition(run, idle, false, "IsMoving", AnimatorConditionMode.IfNot);

        AddTriggerTransition(stateMachine, attack, "Attack");
        AddExitConditionTransition(attack, run, "IsMoving", AnimatorConditionMode.If);
        AddExitConditionTransition(attack, idle, "IsMoving", AnimatorConditionMode.IfNot);

        AddTriggerTransition(stateMachine, die, "Die");

        AddTriggerTransition(stateMachine, jumpReady, "Jump");
        AddExitTransition(jumpReady, jumpUp);

        AddConditionTransition(jumpUp, jumpDown, false, "YVelocity", AnimatorConditionMode.Less, -0.1f);
        AddGroundedMoveTransition(jumpDown, run, true);
        AddGroundedMoveTransition(jumpDown, idle, false);

        AnimatorStateTransition toJumpDown = stateMachine.AddAnyStateTransition(jumpDown);
        ConfigureTransition(toJumpDown, false);
        toJumpDown.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsGrounded");
        toJumpDown.AddCondition(AnimatorConditionMode.Less, -0.1f, "YVelocity");
    }

    private static AnimatorState AddState(
        AnimatorController controller,
        AnimatorStateMachine stateMachine,
        string stateName,
        Vector3 position)
    {
        AnimatorState state = stateMachine.AddState(stateName, position);
        AnimationClip placeholder = new AnimationClip { name = stateName };
        AssetDatabase.AddObjectToAsset(placeholder, controller);
        state.motion = placeholder;
        return state;
    }

    private static void AddTriggerTransition(AnimatorStateMachine stateMachine, AnimatorState target, string parameter)
    {
        AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(target);
        ConfigureTransition(transition, false);
        transition.AddCondition(AnimatorConditionMode.If, 0f, parameter);
    }

    private static void AddTriggerTransition(AnimatorState source, AnimatorState target, string parameter)
    {
        AnimatorStateTransition transition = source.AddTransition(target);
        ConfigureTransition(transition, false);
        transition.AddCondition(AnimatorConditionMode.If, 0f, parameter);
    }

    private static void AddConditionTransition(
        AnimatorState source,
        AnimatorState target,
        bool hasExitTime,
        string parameter,
        AnimatorConditionMode mode,
        float threshold = 0f)
    {
        AnimatorStateTransition transition = source.AddTransition(target);
        ConfigureTransition(transition, hasExitTime);
        transition.AddCondition(mode, threshold, parameter);
    }

    private static void AddExitTransition(AnimatorState source, AnimatorState target)
    {
        AnimatorStateTransition transition = source.AddTransition(target);
        ConfigureTransition(transition, true);
    }

    private static void AddExitConditionTransition(
        AnimatorState source,
        AnimatorState target,
        string parameter,
        AnimatorConditionMode mode)
    {
        AnimatorStateTransition transition = source.AddTransition(target);
        ConfigureTransition(transition, true);
        transition.AddCondition(mode, 0f, parameter);
    }

    private static void AddGroundedMoveTransition(AnimatorState source, AnimatorState target, bool isMoving)
    {
        AnimatorStateTransition transition = source.AddTransition(target);
        ConfigureTransition(transition, false);
        transition.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
        transition.AddCondition(isMoving ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0f, "IsMoving");
    }

    private static void ConfigureTransition(AnimatorStateTransition transition, bool hasExitTime)
    {
        transition.hasExitTime = hasExitTime;
        transition.exitTime = hasExitTime ? 1f : 0f;
        transition.duration = 0f;
        transition.hasFixedDuration = true;
        transition.canTransitionToSelf = false;
    }
}
