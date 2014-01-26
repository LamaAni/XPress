using System;
using System.Reflection;

public delegate void UnregisterCallback<E>(EventHandler<E> eventHandler)
  where E : EventArgs;

public interface IWeakEvent {
    Delegate OpenHandler { get; }
}

public interface IWeakEventHandler<E>: IWeakEvent
  where E : EventArgs
{
    EventHandler<E> Handler { get; }
}

public class WeakEventHandler<T, E> : IWeakEventHandler<E>
    where T : class
    where E : EventArgs
{
    private delegate void OpenEventHandler(T @this, object sender, E e);

    private WeakReference m_TargetRef;
    private OpenEventHandler m_OpenHandler;

    public Delegate OpenHandler
    {
        get { return m_OpenHandler; }
    }

    private EventHandler<E> m_Handler;
    private UnregisterCallback<E> m_Unregister;

    public WeakEventHandler(EventHandler<E> eventHandler, UnregisterCallback<E> unregister)
    {
        m_TargetRef = new WeakReference(eventHandler.Target);
        m_OpenHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler),
          null, eventHandler.Method);
        m_Handler = Invoke;
        m_Unregister = unregister;
    }

    public void Invoke(object sender, E e)
    {
        T target = (T)m_TargetRef.Target;

        if (target != null)
            m_OpenHandler.Invoke(target, sender, e);
        else if (m_Unregister != null)
        {
            m_Unregister(m_Handler);
            m_Unregister = null;
        }
    }

    public EventHandler<E> Handler
    {
        get { return m_Handler; }
    }

    public static implicit operator EventHandler<E>(WeakEventHandler<T, E> weh)
    {
        return weh.m_Handler;
    }
}

public delegate void UnregisterCallback(EventHandler eventHandler);

public interface IWeakEventHandler : IWeakEvent
{
    EventHandler Handler { get; }
}

public class WeakEventHandler<T> : IWeakEventHandler 
    where T : class
{
    private delegate void OpenEventHandler(T @this, object sender, EventArgs e);

    private WeakReference m_TargetRef;
    private OpenEventHandler m_OpenHandler;

    /// <summary>
    /// The handler for the function that will be invoked.
    /// </summary>
    public Delegate OpenHandler
    {
        get { return m_OpenHandler; }
    }

    private EventHandler  m_Handler;
    private UnregisterCallback  m_Unregister;

    public WeakEventHandler(EventHandler  eventHandler, UnregisterCallback  unregister)
    {
        m_TargetRef = new WeakReference(eventHandler.Target);
        m_OpenHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler),
          null, eventHandler.Method);
        m_Handler = Invoke;
        m_Unregister = unregister;
    }

    public void Invoke(object sender, EventArgs e)
    {
        T target = (T)m_TargetRef.Target;

        if (target != null)
            m_OpenHandler.Invoke(target, sender, e);

        else if (m_Unregister != null)
        {
            m_Unregister(m_Handler);
            m_Unregister = null;
        }
    }

    public EventHandler Handler
    {
        get { return m_Handler; }
    }

    public static implicit operator EventHandler (WeakEventHandler<T> weh)
    {
        return weh.m_Handler;
    }
}

public static class WEvent
{
    public static EventHandler<E> Make<E>(EventHandler<E> eventHandler)
      where E : EventArgs
    {
        UnregisterCallback<E> unregister = new UnregisterCallback<E>(delegate(EventHandler<E> eh)
          {
          });

        if (eventHandler == null)
            throw new ArgumentNullException("eventHandler");
        if (eventHandler.Method.IsStatic || eventHandler.Target == null)
            throw new ArgumentException("Only instance methods are supported.", "eventHandler");

        Type wehType = typeof(WeakEventHandler<,>).MakeGenericType(eventHandler.Method.DeclaringType, typeof(E));
            ConstructorInfo wehConstructor = wehType.GetConstructor(new Type[] { typeof(EventHandler<E>),
            typeof(UnregisterCallback<E>) });

        IWeakEventHandler<E> weh = (IWeakEventHandler<E>)wehConstructor.Invoke(
            new object[] { eventHandler, unregister });

        return weh.Handler;
    }

    public static EventHandler Make(EventHandler eventHandler)
    {
        UnregisterCallback unregister = new UnregisterCallback(delegate(EventHandler eh)
          {
          });

        if (eventHandler == null)
            throw new ArgumentNullException("eventHandler");

        if (eventHandler.Method.IsStatic || eventHandler.Target == null)
            throw new ArgumentException("Only instance methods are supported.", "eventHandler");

        Type wehType = typeof(WeakEventHandler<>).MakeGenericType(
            eventHandler.Method.DeclaringType);

        ConstructorInfo wehConstructor = wehType.GetConstructor(new Type[] {typeof(EventHandler),
            typeof(UnregisterCallback) });

        IWeakEventHandler weh = (IWeakEventHandler)wehConstructor.Invoke(
            new object[] { eventHandler, unregister });

        return weh.Handler;
    }

}