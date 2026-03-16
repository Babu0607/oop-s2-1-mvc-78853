
using Xunit;
using Library.Domain;

namespace Library.Tests;

public class BookUnitTest
{
    [Fact]
    public void CanCreateBook()
    {
        var book = new Book
        {
            Id = 1,
            Title = "Test Book",
            Author = "Test Author",
            Isbn = "1234567890123",
            Category = "Fiction",
            IsAvailable = true
        };

        Assert.Equal(1, book.Id);
        Assert.Equal("Test Book", book.Title);
        Assert.Equal("Test Author", book.Author);
        Assert.Equal("1234567890123", book.Isbn);
        Assert.Equal("Fiction", book.Category);
        Assert.True(book.IsAvailable);
    }

    [Fact]
    public void Book_IsAvailable_DefaultTrue()
    {
        var book = new Book();

        Assert.True(book.IsAvailable);
    }

    [Fact]
    public void Book_CanBeUnavailable()
    {
        var book = new Book();
        
        book.IsAvailable = false;

        Assert.False(book.IsAvailable);
    }
}