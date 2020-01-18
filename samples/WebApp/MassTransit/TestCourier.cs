using System;
using System.Threading.Tasks;
using MassTransit.Courier;

namespace WebApp.MassTransit
{
    public class DownloadImageActivity : IActivity<DownloadImageArguments, DownloadImageLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<DownloadImageArguments> context)
        {
            return default;
        }

        public async Task<CompensationResult> Compensate(CompensateContext<DownloadImageLog> context)
        {
            return default;
        }
    }

    public class FilterImageActivity : IActivity<FilterImageArguments, FilterImageLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<FilterImageArguments> context)
        {
            return default;
        }

        public async Task<CompensationResult> Compensate(CompensateContext<FilterImageLog> context)
        {
            return default;
        }
    }

    public class FilterImageLog
    {
    }

    public class FilterImageArguments
    {
    }

    public class DownloadImageLog
    {
    }

    public class DownloadImageArguments
    {
    }
}
