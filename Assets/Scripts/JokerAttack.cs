public class JokerAttack : Attack 
{
    protected Cocos2dAction atkJumpAction;
    public override void AtkAnimation()
    {
        guard.FaceTarget(guard.spot.target);
        if (!checkIfTargetPressDown())
        {
            if (guard.spriteSheet.HasAnimation("kick") && Globals.stealingController != null && Globals.stealingController.magician.currentAction == Globals.stealingController.magician.catchByNet)
            {
                guard.spriteSheet.Play("kick");
            }
            else
            {
                guard.spriteSheet.Play("Atk");
                atkJumpAction = new Sequence(new JumpTo(transform, guard.spot.target.transform.position, 150.0f,
guard.spriteSheet.GetAnimationLengthWithSpeed("Atk") + 15), new FunctionCall(() => AtkEnd()));
                guard.AddAction(atkJumpAction);
                guard.spriteRenderer.gameObject.layer = 21;
            }
        }
        else
        {
            if (guard.spriteSheet.HasAnimation("atkReady"))
            {
                guard.spriteSheet.Play("atkReady");
            }
            else
            {
                guard.spriteSheet.Play("idle");
            }
        }
    }
    public override void Stop()
    {
        base.Stop();

        if (atkJumpAction != null)
        {
            guard.RemoveAction(ref atkJumpAction);
        }

        guard.spriteRenderer.gameObject.layer = 13;
    }

    public override void AtkEnd()
    {
        base.AtkEnd();
        atkJumpAction = null;
        guard.gameObject.layer = 13;
    }
}