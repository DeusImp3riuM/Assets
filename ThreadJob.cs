using UnityEngine;
using System.Collections;

public class ThreadJob{
	
	private bool _isDone = false;
	private bool _isEnded = false;
	private object _Handle = new object ();
	private System.Threading.Thread _Thread = null;

	public bool isDone{
		get{
			bool tmp;
			lock(_Handle){
				tmp = _isDone;
			}
			return tmp;
		}
		set{
			lock(_Handle){
				_isDone = value;
			}
		}
	}

	public virtual void Start(){
		
		_Thread = new System.Threading.Thread (Run);
		_isEnded = false;
		_Thread.Start ();
	}

	public virtual void About(){
		_Thread.Abort ();
	}

	protected virtual void ThreadFunction(){}

	protected virtual void OnFinished(){}


	public virtual bool Update(){
		if(isDone){
			OnFinished ();
			return true;
		}
		return false;
	}
	/*
	public virtual void Update(){
		if(isDone && !_isEnded){
			OnFinished ();
			_isEnded = true;
		}
	}
	*/
	public bool Ended{
		get{
			return _isEnded;
		}
	}
	/*
	public IEnumerator WaitFor(){
		while(!Update()){
			yield return null;
		}
	}
	*/
	private void Run(){
		ThreadFunction ();
		isDone = true;
	}
}
