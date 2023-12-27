namespace _02350AdvancedDemo.Service;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Circle, CircleViewModel>();
        CreateMap<CircleViewModel, Circle>();
        CreateMap<Square, SquareViewModel>();
        CreateMap<SquareViewModel, Square>();

        CreateMap<Line, LineViewModel>();
        CreateMap<LineViewModel, Line>();
        CreateMap<DashLine, DashLineViewModel>();
        CreateMap<DashLineViewModel, DashLine>();
    }
}
