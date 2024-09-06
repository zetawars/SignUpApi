using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class AuthenticateCommand : IRequest<AuthenticatedResponse>
    {
        public AuthenticateRequest Request { get; set; }
    }

    public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, AuthenticatedResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticateCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthenticatedResponse> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Request.Username);

            if (user == null || !_passwordHasher.VerifyPassword(request.Request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            await _userRepository.RecordLoginAsync(user.Id, request.Request.IpAddress, request.Request.Device, request.Request.Browser);

            return new AuthenticatedResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token
            };
        }
    }
}
