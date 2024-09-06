using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Users.Querries
{
    public class GetBalanceQuery : IRequest<BalanceResponse>
    {
        public string Token { get; set; }
    }

    public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, BalanceResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public GetBalanceQueryHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<BalanceResponse> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            var userId = _jwtTokenGenerator.ValidateToken(request.Token);
            var user = await _userRepository.GetUserByIdAsync(userId);

            return new BalanceResponse
            {
                Balance = user.Balance
            };
        }
    }

}
