public interface IPolicyService
{
    Task<List<PolicyDTO>> GetAllPoliciesAsync();
    Task<PolicyDTO> GetPolicyByIdAsync(Guid policyId);
    Task<bool> AddPolicyAsync(PolicyDTO policyDTO);
    Task<bool> UpdatePolicyAsync(Guid policyId, PolicyDTO policyDTO);
    Task<bool> DeletePolicyAsync(Guid policyId);
}
