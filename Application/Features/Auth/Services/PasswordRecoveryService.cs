using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Domain;
using Domain.Entities;

namespace Application.Features.Auth.Services;

public class PasswordRecoveryService:IPasswordRecoveryService
{
    private readonly IHasher _hasher;
    private readonly UserRepositoryContract _userRepositoryContract;
    public PasswordRecoveryService(IHasher hasher, UserRepositoryContract userRepositoryContract)
    {
        _hasher = hasher;
        _userRepositoryContract = userRepositoryContract;
    }
    
    // public async Task ResetPasswordAsync(string email,string code,string newPassword)
    // {
    //     var user=await _userRepositoryContract.GetUserByEmailAsync(email);
    //     if (user == null)
    //         throw new InvalidOperationException("Invalid request");
    //     
    //     var codeHash=_hasher.Hash(code);
    //
    //     if (!user.CanUseResetPassword(codeHash, DateTime.UtcNow))
    //     {
    //         user.IncreasePasswordResetAttemptCount();
    //         await _userRepositoryContract.SaveChangesAsync();
    //         throw new InvalidOperationException("Incorrect or expired code");
    //     }
    //     
    //     var hashedPassword = _hasher.Hash(newPassword);
    //     user.ResetPassword(hashedPassword, DateTime.UtcNow);
    //     user.ClearPasswordResetCode();
    //     
    //     await _userRepositoryContract.SaveChangesAsync();
    // }
    
    // public async Task ForgetPasswordAsync(string email)
    // {
    //     var user = await _userRepositoryContract.GetUserByEmailAsync(email);
    //     if (user == null)
    //         throw new NotFoundException($"user with email : {email} not found"); 
    //
    //     var code = _codeGenerator.Generate6DigitCode();
    //     var codeHash = _hasher.Hash(code);
    //
    //     user.ResetPassword(
    //         codeHash,
    //         DateTime.UtcNow.AddMinutes(5)
    //     );
    //
    //     await _userRepositoryContract.SaveChangesAsync();
    //     
    //     //Implement Email Service
    //     
    //     // await _emailService.SendAsync(
    //     //     email,
    //     //     "Password Reset Code",
    //     //     $"Your verification code is: {code}"
    //     // );
    // }
}