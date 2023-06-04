using EJournal.Response;

namespace EJournal.Service.Interfaces
{
    public interface IDumpService
    {
        Task<BaseResponse<string[]>> GetBackUpsName();

		Task CreateDump();
		Task DeleteDump(string dumpName);
		Task<BaseResponse<string>> JumpDump(string dumpName);

	}
}
