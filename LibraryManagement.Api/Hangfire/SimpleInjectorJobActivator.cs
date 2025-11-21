using Hangfire;
using SimpleInjector;

namespace LibraryManagement.Api.Hangfire;

public class SimpleInjectorJobActivator : JobActivator
{
    private readonly Container _container;

    public SimpleInjectorJobActivator(Container container)
    {
        _container = container;
    }

    public override object ActivateJob(Type jobType)
    {
        return _container.GetInstance(jobType);
    }
}
