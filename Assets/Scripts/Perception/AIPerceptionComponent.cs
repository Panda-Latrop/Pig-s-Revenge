using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPerceptionTarget
{
    Pawn Self { get; }
}

public struct StimulusStruct
{
    public bool enter;
    public Pawn target;
    public bool neverLose;

    public StimulusStruct(bool enter, Pawn target)
    {
        this.enter = enter;
        this.target = target;
        neverLose = false;
    }
    public StimulusStruct(bool enter, Pawn target,bool neverLose)
    {
        this.enter = enter;
        this.target = target;
        this.neverLose = neverLose;
    }
}

public delegate void PerceptionDelegate(StimulusStruct stimulus);
public class AIPerceptionComponent : ActorComponent
{
    [SerializeField]
    protected bool useDetector = true;
    [SerializeField]
    protected Collider2D detector;
    protected Pawn owner;
    protected int bindedOnPerceptionDetection;
    protected Queue<StimulusStruct> slowStimulus = new Queue<StimulusStruct>();

    protected PerceptionDelegate OnPerceptionDetection;

    protected void OnEnable()
    {
        if(useDetector)
        detector.enabled = true;
    }
    protected void OnDisable()
    {
        if (useDetector)
            detector.enabled = false;
    }

    public void ForceDetection(Pawn pawn)
    {
        StimulusStruct stimulus = new StimulusStruct(true, pawn);
        if (bindedOnPerceptionDetection > 0)
            CallOnPerceptionDetection(stimulus);
        else
            slowStimulus.Enqueue(stimulus);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        IPerceptionTarget pawn = collision.GetComponent<IPerceptionTarget>();
        if (pawn != null)
        {
            StimulusStruct stimulus = new StimulusStruct(true, pawn.Self);
                            if (bindedOnPerceptionDetection > 0)
                CallOnPerceptionDetection(stimulus);
            else
                slowStimulus.Enqueue(stimulus);
        }
    }
    protected void OnTriggerExit2D(Collider2D collision)
    {
        IPerceptionTarget pawn = collision.GetComponent<IPerceptionTarget>();
        if (pawn != null)
        {
            StimulusStruct stimulus = new StimulusStruct(false, pawn.Self);
            if (bindedOnPerceptionDetection > 0)
                CallOnPerceptionDetection(stimulus);
            else
                slowStimulus.Enqueue(stimulus);
        }
    }

    protected override void OnDestroyLate()
    {
        OnPerceptionDetection = null;
        bindedOnPerceptionDetection = 0;
    }
    protected void OnDestroy()
    {
        OnDestroyLate();
    }

    #region Delegate
    public void CallOnPerceptionDetection(StimulusStruct stimulus)
    {
        OnPerceptionDetection?.Invoke(stimulus);
    }
    public void BindOnPerceptionDetection(PerceptionDelegate action)
    {

        OnPerceptionDetection += action;
        bindedOnPerceptionDetection++;
        while (slowStimulus.Count > 0)
        {
            CallOnPerceptionDetection(slowStimulus.Dequeue());
        }

    }
    public void UnbindOnPerceptionDetection(PerceptionDelegate action)
    {
        OnPerceptionDetection -= action;
        bindedOnPerceptionDetection--;
    }
    public void ClearOnPerceptionDetection()
    {
        OnPerceptionDetection = null;
        bindedOnPerceptionDetection = 0;
    }
    #endregion
}
