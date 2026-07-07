namespace Domain;

public interface IHasher
{
    string Hash(string input);
    bool Verify(string input, string hash);
}