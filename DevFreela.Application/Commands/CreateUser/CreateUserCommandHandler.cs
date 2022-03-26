using DevFreela.Core.Entities;
using DevFreela.Core.Repositories;
using DevFreela.Core.Services;
using DevFreela.Infrastructure.Persistence;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DevFreela.Application.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly IUserRepository _userRepository;
        private IAuthService _authService;
        public CreateUserCommandHandler(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var passwordHash = _authService.ComputeSha256Hash(request.Password);
            var user = new User(request.FullName, request.Email, request.BirthDate, passwordHash, request.Role);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user.Id;
        }
    }
}
