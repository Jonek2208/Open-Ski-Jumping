using System;
using UnityEngine.Events;

[Serializable]
public class UnityEventInt : UnityEvent<int> { }

[Serializable]
public class UnityEventFloat : UnityEvent<float> { }

[Serializable]
public class UnityEventDecimal : UnityEvent<decimal> { }

[Serializable]
public class UnityEventBool : UnityEvent<bool> { }

[Serializable]
public class UnityEventString : UnityEvent<string> { }
