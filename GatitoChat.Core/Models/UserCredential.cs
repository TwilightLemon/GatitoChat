namespace GatitoChat.Core.Models;

public record class UserCredential(
    string Username,string BlindedUid,
    string Token,string RandomSeed,string Sign
    );