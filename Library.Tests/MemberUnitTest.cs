using Xunit;
using Library.Domain;

namespace Library.Tests;

public class MemberUnitTest
{
    [Fact]
    public void CanCreateMember()
    {
        var member = new Member
        {
            Id = 1,
            FullName = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "123-456-7890"
        };

        Assert.Equal(1, member.Id);
        Assert.Equal("John Doe", member.FullName);
        Assert.Equal("john@example.com", member.Email);
        Assert.Equal("123-456-7890", member.PhoneNumber);
    }

    [Fact]
    public void Member_CanHaveMultipleLoans()
    {
        var member = new Member
        {
            FullName = "Jane Smith",
            Email = "jane@example.com"
        };

        var loan1 = new Loan { Id = 1 };
        var loan2 = new Loan { Id = 2 };

        member.Loans.Add(loan1);
        member.Loans.Add(loan2);

        Assert.Equal(2, member.Loans.Count);
    }
}