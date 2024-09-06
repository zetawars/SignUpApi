using Application.CustomExceptions;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class SignupCommand : IRequest<Unit>
    {
        public SignupRequest Request { get; set; }
    }

    public class SignupCommandHandler : IRequestHandler<SignupCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public SignupCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(SignupCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Request.Username);
            if (existingUser != null)
            {
                throw new UserAlreadyExistsException(existingUser.Username);
            }

            var user = new User
            {
                Username = request.Request.Username,
                PasswordHash = _passwordHasher.HashPassword(request.Request.Password),
                FirstName = request.Request.FirstName,
                LastName = request.Request.LastName,
                Device = request.Request.Device,
                IpAddress = request.Request.IpAddress,
                CreatedAt = DateTime.UtcNow,
                Balance = 5m
            };

            await _userRepository.AddUserAsync(user);
            return Unit.Value;
        }
    }
}
