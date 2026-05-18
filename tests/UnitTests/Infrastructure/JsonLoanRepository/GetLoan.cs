using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Library.UnitTests.Infrastructure.JsonLoanRepository;

public class GetLoanTest
{
	private readonly ILoanRepository _mockLoanRepository;
	private readonly Library.Infrastructure.Data.JsonLoanRepository _jsonLoanRepository;
	private readonly IConfiguration _configuration;
	private readonly JsonData _jsonData;

	public GetLoanTest()
	{
		_mockLoanRepository = Substitute.For<ILoanRepository>();
		var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

		_configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["JsonPaths:Authors"] = Path.Combine(repoRoot, "src", "Library.Console", "Json", "Authors.json"),
				["JsonPaths:Books"] = Path.Combine(repoRoot, "src", "Library.Console", "Json", "Books.json"),
				["JsonPaths:BookItems"] = Path.Combine(repoRoot, "src", "Library.Console", "Json", "BookItems.json"),
				["JsonPaths:Patrons"] = Path.Combine(repoRoot, "src", "Library.Console", "Json", "Patrons.json"),
				["JsonPaths:Loans"] = Path.Combine(repoRoot, "src", "Library.Console", "Json", "Loans.json")
			})
			.Build();

		_jsonData = new JsonData(_configuration);
		_jsonLoanRepository = new Library.Infrastructure.Data.JsonLoanRepository(_jsonData);
	}

	[Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns loan when loan ID is found")]
	public async Task GetLoan_ReturnsLoan_WhenLoanIdFound()
	{
		// Arrange
		var loanId = 1;
		var expectedLoan = new Loan { Id = loanId };
		_mockLoanRepository.GetLoan(loanId).Returns(expectedLoan);

		// Act
		var expected = await _mockLoanRepository.GetLoan(loanId);
		var actual = await _jsonLoanRepository.GetLoan(loanId);

		// Assert
		Assert.NotNull(expected);
		Assert.NotNull(actual);
		Assert.Equal(expected!.Id, actual!.Id);
	}
    // create a test for the case where the loan ID isn’t found
    [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns null when loan ID is not found")]
    public async Task GetLoan_ReturnsNull_WhenLoanIdNotFound()
    {
        // Arrange
        var loanId = 999; // Assuming this ID does not exist
        _mockLoanRepository.GetLoan(loanId).Returns((Loan?)null);

        // Act
        var expected = await _mockLoanRepository.GetLoan(loanId);
        var actual = await _jsonLoanRepository.GetLoan(loanId);

        // Assert
        Assert.Null(expected);
        Assert.Null(actual);
    }
}
