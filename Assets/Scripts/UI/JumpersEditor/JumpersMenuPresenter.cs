using System.Linq;
using Competition;

public class JumpersMenuPresenter
{
    private IJumpersMenuView view;
    private CompetitorsRuntime jumpers;
    private FlagsData flagsData;

    public JumpersMenuPresenter(IJumpersMenuView view, CompetitorsRuntime jumpers, FlagsData flagsData)
    {
        this.view = view;
        this.jumpers = jumpers;
        this.flagsData = flagsData;

        InitEvents();
        SetInitValues();
    }

    private void CreateNewJumper()
    {
        Competitor jumper = new Competitor("", "", "");
        jumpers.Add(jumper);
        PresentList();
        view.SelectJumper(jumper);
    }

    private void RemoveJumper()
    {
        Competitor jumper = view.SelectedJumper;
        if (jumper == null) { return; }
        bool val = jumpers.Remove(jumper);

        PresentList();
        PresentJumperInfo();
    }

    private void PresentList()
    {
        view.Jumpers = jumpers.GetData();
    }

    private void PresentJumperInfo()
    {
        var jumper = view.SelectedJumper;
        if (jumper == null) { view.HideJumperInfo(); return; }
        view.ShowJumperInfo();
        view.FreezeJumperInfoCallbacks = true;

        view.FirstName = jumper.firstName;
        view.LastName = jumper.lastName;
        view.CountryCode = jumper.countryCode;
        view.SuitTopFront = jumper.suitTopFrontColor;
        view.SuitTopBack = jumper.suitTopBackColor;
        view.SuitBottomFront = jumper.suitBottomFrontColor;
        view.SuitBottomBack = jumper.suitBottomBackColor;
        view.Helmet = jumper.helmetColor;
        view.Skis = jumper.skisColor;
        view.ImagePath = jumper.imagePath;
        view.LoadImage(jumper.imagePath);
        view.FreezeJumperInfoCallbacks = false;
    }

    private void SaveJumperInfo()
    {
        var jumper = view.SelectedJumper;
        if (jumper == null) { return; }

        jumper.firstName = view.FirstName;
        jumper.lastName = view.LastName;
        jumper.countryCode = view.CountryCode;
        jumper.suitTopFrontColor = view.SuitTopFront;
        jumper.suitTopBackColor = view.SuitTopBack;
        jumper.suitBottomFrontColor = view.SuitBottomFront;
        jumper.suitBottomBackColor = view.SuitBottomBack;
        jumper.helmetColor = view.Helmet;
        jumper.skisColor = view.Skis;
        jumper.imagePath = view.ImagePath;
        jumpers.Recalculate(jumper);
        PresentList();
        view.SelectJumper(jumper);
    }
    private void InitEvents()
    {
        view.OnSelectionChanged += PresentJumperInfo;
        view.OnAdd += CreateNewJumper;
        view.OnRemove += RemoveJumper;
        view.OnCurrentJumperChanged += SaveJumperInfo;
    }

    private void SetInitValues()
    {
        PresentList();
        PresentJumperInfo();
    }
}
