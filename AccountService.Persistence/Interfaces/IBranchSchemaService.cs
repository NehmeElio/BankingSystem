namespace AccountService.Persistence.Interfaces;

public interface IBranchSchemaService
{
   void CreateBranchSchema(string newBranchName, string sourceSchemaName = "beirutbranch");
}