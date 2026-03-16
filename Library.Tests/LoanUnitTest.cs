using System;
using Xunit;
using Library.Domain;

namespace Library.Tests;

public class LoanUnitTest
{
    [Fact]
    public void CanCreateLoan()
    {
        var loan = new Loan
        {
            Id = 1,
            BookId = 10,
            MemberId = 20,
            LoanDate = new DateTime(2024, 3, 1),
            DueDate = new DateTime(2024, 3, 15),
            ReturnedDate = null
        };

        Assert.Equal(1, loan.Id);
        Assert.Equal(10, loan.BookId);
        Assert.Equal(20, loan.MemberId);
        Assert.Equal(new DateTime(2024, 3, 1), loan.LoanDate);
        Assert.Equal(new DateTime(2024, 3, 15), loan.DueDate);
        Assert.Null(loan.ReturnedDate);
    }

    [Fact]
    public void Loan_IsOverdue_WhenDueDatePassedAndNotReturned()
    {
        var loan = new Loan
        {
            LoanDate = DateTime.Today.AddDays(-20),
            DueDate = DateTime.Today.AddDays(-5),
            ReturnedDate = null
        };

        Assert.True(loan.DueDate < DateTime.Today && !loan.ReturnedDate.HasValue);
    }

    [Fact]
    public void Loan_IsNotOverdue_WhenReturned()
    {
        var loan = new Loan
        {
            LoanDate = DateTime.Today.AddDays(-30),
            DueDate = DateTime.Today.AddDays(-15),
            ReturnedDate = DateTime.Today.AddDays(-10)
        };

        Assert.False(loan.DueDate < DateTime.Today && !loan.ReturnedDate.HasValue);
    }

    [Fact]
    public void Loan_IsNotOverdue_WhenDueDateInFuture()
    {
        var loan = new Loan
        {
            LoanDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(14),
            ReturnedDate = null
        };

        Assert.False(loan.DueDate < DateTime.Today);
    }
}