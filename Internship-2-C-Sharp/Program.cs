using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Transactions;
using System.Xml.Linq;

DateTime NewDate()
{
    DateTime Date;
    bool correct = DateTime.TryParse(Console.ReadLine(), out Date);
    while (!correct)
    {
        Console.WriteLine("Neispavan unos datuma, pokušajte ponovo: (YYYY-MM-DD)");
        correct = DateTime.TryParse(Console.ReadLine(), out Date);
    }
    return Date;
}

int UserFunction(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary)
{
    int UserInput = -1;

    do
    {
        Console.WriteLine("\n1 - Korisnici:");
        Console.WriteLine(" 1 - Unos novog korisnka");
        Console.WriteLine(" 2 - Brisanje korisnika");
        Console.WriteLine(" 3 - Uređivanje korisnika");
        Console.WriteLine(" 4 - Pregled korisnika");
        Console.WriteLine(" 5 - Povratak na predhodni izbornik");

        int.TryParse(Console.ReadLine(), out UserInput);

        if (UserInput == 5)
        {
            break;
        }
        else if (UserInput == 1)
        {
            AddNewUser(UserDictionary);
        }
        else if (UserInput == 2)
        {
            DeleteUser(UserDictionary);
        }
        else if (UserInput == 3)
        {
            EditUser(UserDictionary);
        }
        else if (UserInput == 4)
        {
            UserOverview(UserDictionary);
        }
        else
        {
            Console.WriteLine("Nesipravan unos pokušaj ponovo.");
        }
    }
    while (UserInput != 5);
    return 0;
}

void AddNewUser(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary)
{
    string Name = " ";
    string Surname = "";
    DateTime UserDate;

    Console.WriteLine(" \n1 - Unos novog korisnka");
    Console.WriteLine("Unesi ime korisnika: ");
    Name = Console.ReadLine();

    while (Name == "" || Name == " ")
    {
        Console.WriteLine("Ime ne moze biti prazno!");
        Name = Console.ReadLine();
    }
    Console.WriteLine("Unesite prezime korisnika: ");
    Surname = Console.ReadLine();

    while (Surname == "" || Surname == " ")
    {
        Console.WriteLine("Prezime ne moze biti prazno!");
        Surname = Console.ReadLine();

    }

    Console.WriteLine("Unesi Datum Rođenja: (YYYY-MM-DD)");
    bool correct = DateTime.TryParse(Console.ReadLine(), out UserDate);
    while (!correct)
    {
        Console.WriteLine("Neispavan unos datuma, pokušajte ponovo: (YYYY-MM-DD)");
        correct = DateTime.TryParse(Console.ReadLine(), out UserDate);
        ;
    }


    UserDictionary[Name.GetHashCode()] = (Name, Surname, UserDate, 100, 0, 0, new Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)>());
    Console.WriteLine("Korisnik uspjesno dodan.\n");
}

void DeleteUser(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary)
{
    Console.WriteLine("Po ceme brišete korisnika: ");
    Console.WriteLine("a - id-u");
    Console.WriteLine("b - imenu i prezimenu");
    string DeleteOption;
    int DeleteUserID;

    DeleteOption = Console.ReadLine();
    DeleteOption = DeleteOption.ToLower();

    while (DeleteOption != "a" && DeleteOption != "b")
    {
        Console.WriteLine("Neispravan unos, unesite a ili b!");
        DeleteOption = Console.ReadLine();
        DeleteOption = DeleteOption.ToLower();
    }

    if (DeleteOption == "a")
    {
        Console.WriteLine("Unesi ID korisnika kojeg zelite izbrisati:");
        int.TryParse(Console.ReadLine(), out DeleteUserID);
    }
    else
    {
        Console.WriteLine("Unesite ime korisnika kojeg zelite izbrisati: ");
        string Name = Console.ReadLine();
        while (Name == "" || Name == " ")
        {
            Console.WriteLine("Unesite ime korisnika kojeg zelite izbrisati: ");
            Name = Console.ReadLine();
        }
        Console.WriteLine("Unesite prezime korisnika kojeg zelite izbrisati: ");
        string Surname = Console.ReadLine();

        while (Surname == "" || Surname == " ")
        {
            Console.WriteLine("Unesite ime korisnika kojeg zelite izbrisati: ");
            Surname = Console.ReadLine();
        }

        DeleteUserID = FindUser(UserDictionary, Name.ToLower(), Surname.ToLower());
    }

    if (UserDictionary.ContainsKey(DeleteUserID))
    {
        Console.WriteLine($"Sigurni ste da zelite izbrisati korisnika: {DeleteUserID} - {UserDictionary[DeleteUserID].Name}- {UserDictionary[DeleteUserID].Name} (Y/N)");
        string stringYN = Console.ReadLine().ToLower();
        while (stringYN != "y" && stringYN != "n")
        {
            stringYN = Console.ReadLine();
            stringYN = stringYN.ToLower();
        }
        if (stringYN == "y")
        {
            UserDictionary.Remove(DeleteUserID);
        }
        else
        {
            Console.WriteLine("Ponisteno");
        }
    }
    else
    {
        Console.WriteLine($"Nepostoji odabrani korisnik.");
    }
}

int FindUser(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary, string Name, string Surname)
{
    int TargetUserID = -1;
    foreach (var elemnet in UserDictionary)
    {
        if (elemnet.Value.Name.ToLower() == Name && elemnet.Value.Surname.ToLower() == Surname)
        {
            TargetUserID = elemnet.Key;
        }
    }
    return TargetUserID;
}

void EditUser(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary)
{
    int TargetUserId;
    string NewName = "";
    string NewSurname = "";
    DateTime NewDatee;
    string stringYN;

    Console.WriteLine("Unesi ID korisnika kojeg zelite urediti:");
    int.TryParse(Console.ReadLine(), out TargetUserId);

    if (!UserDictionary.ContainsKey(TargetUserId))
    {
        Console.WriteLine($"Korisni sa ID {TargetUserId} ne postoji");
        return;
    }
    Console.WriteLine($"Zelite urediti korisnika: {TargetUserId}-{UserDictionary[TargetUserId].Name}-{UserDictionary[TargetUserId].Surname}-{UserDictionary[TargetUserId].Date.ToString("yyyy-MM-dd")}");
    Console.WriteLine("Zelite li urediti ime? Y/N");
    stringYN = Console.ReadLine();
    stringYN = stringYN.ToLower();
    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine();
        stringYN = stringYN.ToLower();
    }
    if (stringYN == "y")
    {
        Console.WriteLine("Unesite novo ime:");
        NewName = Console.ReadLine();
        while (NewName == "" || NewName == " ")
        {
            Console.WriteLine("Neispravno ime, ponovi unos: ");
            NewName = Console.ReadLine();
        }
    }
    else
    {
        NewName = UserDictionary[TargetUserId].Name;
    }
    stringYN = "";

    Console.WriteLine("Zelite li urediti prezime? Y/N");
    stringYN = Console.ReadLine();
    stringYN = stringYN.ToLower();
    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine();
        stringYN = stringYN.ToLower();
    }
    if (stringYN == "y")
    {
        Console.WriteLine("Unesite novo prezime:");
        NewSurname = Console.ReadLine();
        while (NewSurname == "" || NewSurname == " ")
        {
            Console.WriteLine("Neispravno prezime, ponovi unos: ");
            NewSurname = Console.ReadLine();
        }
    }
    else
    {
        NewSurname = UserDictionary[TargetUserId].Surname;
    }
    stringYN = "";

    Console.WriteLine("Zelite li urediti datum rodenja? Y/N");
    stringYN = Console.ReadLine();
    stringYN = stringYN.ToLower();
    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine();
        stringYN = stringYN.ToLower();
    }
    if (stringYN == "y")
    {
        Console.WriteLine("Unesite novi datum: (YYYY-MM-DD)");
        NewDatee = NewDate();
    }
    else
    {
        NewDatee = UserDictionary[TargetUserId].Date;
    }
    stringYN = "";

    Console.WriteLine($"Sigurni ste da zelite promjeniti podatke u: {TargetUserId}-{NewName}-{NewSurname}-{NewDatee.ToString("yyyy-MM-dd")}? Y/N");
    stringYN = Console.ReadLine();
    stringYN = stringYN.ToLower();
    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine();
        stringYN = stringYN.ToLower();
    }
    if (stringYN == "y")
    {

        var user = UserDictionary[TargetUserId];

        UserDictionary[TargetUserId] = (
            Name: NewName,
            Surname: NewSurname,
            Date: NewDatee,
            Current: user.Current,
            Checking: user.Checking,
            Prepaid: user.Prepaid,
            TransactionLog: user.TransactionLog
        );
        Console.WriteLine("Podatci uspjesno promjenjeni.");
    }

}

void UserOverview(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary)
{
    Console.WriteLine("Pregled korisnika po: ");
    Console.WriteLine("a - ispis svih korisnika abecedno po prezimenu");
    Console.WriteLine("b - svih onih koji imaju više od 30 godina");
    Console.WriteLine("c - svih onih koji imaju barem jedan račun u minusu");
    string OwerviewOption;

    OwerviewOption = Console.ReadLine();
    OwerviewOption = OwerviewOption.ToLower();

    while (OwerviewOption != "a" && OwerviewOption != "b" && OwerviewOption != "c")
    {
        Console.WriteLine("Neispravan unos, unesite a, b ili c!");
        OwerviewOption = Console.ReadLine();
        OwerviewOption.ToLower();
    }

    if (OwerviewOption == "a")
    {
        Console.WriteLine("\nID-Ime-Prezime-Datum");
        var SortedList = UserDictionary
            .OrderBy(entry => entry.Value.Surname)
            .ToList();
        foreach (var Element in SortedList)
        {
            Console.WriteLine($"{Element.Key}-{UserDictionary[Element.Key].Name}-{UserDictionary[Element.Key].Surname}-{UserDictionary[Element.Key].Date.ToString("yyyy-MM-dd")}");
        }
    }
    else if (OwerviewOption == "b")
    {
        foreach (var Element in UserDictionary)
        {
            int age = DateTime.Today.Year - Element.Value.Date.Year;

            if (Element.Value.Date.Date > DateTime.Today.AddYears(-age)) age--;
            if (age > 30)
            {
                Console.WriteLine($"{Element.Key}-{UserDictionary[Element.Key].Name}-{UserDictionary[Element.Key].Surname}-{UserDictionary[Element.Key].Date.ToString("yyyy-MM-dd")}");
            }
        }
    }
    else
    {
        foreach (var Element in UserDictionary)
        {
            if (Element.Value.Checking < 0 || Element.Value.Current < 0 || Element.Value.Prepaid < 0)
            {
                Console.WriteLine($"{Element.Key}-{UserDictionary[Element.Key].Name}-{UserDictionary[Element.Key].Surname}-{UserDictionary[Element.Key].Date.ToString("yyyy-MM-dd")}");
            }
        }
    }
}


//account Function and it's functions
void AccountFunction(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary)
{
    int UserInput = -1;
    int UserID;

    Console.WriteLine("\n2 - Racuni:");

    Console.WriteLine("Unesite ime korisnika: ");
    string Name = Console.ReadLine();
    Console.WriteLine("Unesite prezime korisnika: ");
    string Surname = Console.ReadLine();

    UserID = FindUser(UserDictionary, Name.ToLower(), Surname.ToLower());

    if (UserDictionary.ContainsKey(UserID))
    {
        SelectAccount(UserDictionary, UserID);
    }
    else
    {
        Console.WriteLine($"Korisnik s nazivom {Name} {Surname} ne postoji!");
    }
}

void SelectAccount(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary, int UserID)
{
    string AccountOption;

    do
    {
        Console.WriteLine("Odaberite racun: ");
        Console.WriteLine("a - tekuci racun");
        Console.WriteLine("b - ziro racun");
        Console.WriteLine("c - prepaid racun");
        Console.WriteLine("d - izlaz");

        AccountOption = Console.ReadLine();
        AccountOption = AccountOption.ToLower();

        while (AccountOption != "a" && AccountOption != "b" && AccountOption != "c" && AccountOption != "d")
        {
            Console.WriteLine("Neispravan unos, unesite a, b,c ili d!");
            AccountOption = Console.ReadLine();
            AccountOption = AccountOption.ToLower();
        }

        if (AccountOption == "a")
        {
            AccountManagement(UserDictionary, UserID, "current");
        }
        else if (AccountOption == "b")
        {
            AccountManagement(UserDictionary, UserID, "checking");
        }
        else if (AccountOption == "c")
        {
            AccountManagement(UserDictionary, UserID, "prepaid");
        }
        else
        {
            break;
        }
    }
    while (AccountOption != "d");
}

void AccountManagement(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary, int UserID, string Account)
{
    int UserInput = -1;

    do
    {
        Console.WriteLine($"\n{UserDictionary[UserID].Name} {UserDictionary[UserID].Surname} {(Account.ToLower() == "current" ? "Tekuci" : "")} {(Account.ToLower() == "prepaid" ? "Prepaid" : "")} {(Account.ToLower() == "checking" ? "ziro" : "")}");
        Console.WriteLine(" 1 - Unos nove transakcije");
        Console.WriteLine(" 2 - Brisanje transakcije");
        Console.WriteLine(" 3 - Uređivanje transakicje");
        Console.WriteLine(" 4 - Pregled transakcija");
        Console.WriteLine(" 5 - Financijsko izvjesce");
        Console.WriteLine(" 6 - Povratak na predhodni izbornik");

        int.TryParse(Console.ReadLine(), out UserInput);

        if (UserInput == 6)
        {
            break;
        }
        else if (UserInput == 1)
        {
            NewTransaction(UserDictionary, UserID, Account);
        }
        else if (UserInput == 2)
        {
            DeleteTransction(UserDictionary[UserID].TransactionLog, Account);
        }
        else if (UserInput == 3)
        {
            EditTransaction(UserDictionary[UserID].TransactionLog, Account);
        }
        else if (UserInput == 4)
        {
            TranscationOverview(UserDictionary[UserID].TransactionLog, Account);
        }
        else if (UserInput == 5)
        {
            FinancialReport(UserDictionary, UserID, Account);
        }
        else
        {
            Console.WriteLine("Nesipravan unos pokušaj ponovo.");
        }
    }
    while (UserInput != 6);
}

void NewTransaction(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary, int UserID, string Account)
{
    string[] CategoryPrihodi = { "plaća", "honorar", "poklon", "dividende", "bonusi" };
    string[] CategoryRashodi = { "hrana", "prijevoz", "sport", "stanarina", "zdravstvo" };

    Console.WriteLine("Nova transakcija.");
    string DateOption;
    DateTime Date = DateTime.Now;
    string Ttype = "";
    float Amount = 0;
    string Description = "Standardna transakcija";
    string Category;

    float AmountOnAcc = 0;

    switch (Account.ToLower())
    {
        case "current":
            AmountOnAcc = UserDictionary[UserID].Current;
            break;
        case "checking":
            AmountOnAcc = UserDictionary[UserID].Checking;
            break;
        case "prepaid":
            AmountOnAcc = UserDictionary[UserID].Prepaid;
            break;
        default:
            break;
    }

    do
    {
        Console.WriteLine("a - trenutna uplata");
        Console.WriteLine("b - prijasnja uplata");

        DateOption = Console.ReadLine();
        DateOption = DateOption.ToLower();

        while (DateOption != "a" && DateOption != "b")
        {
            Console.WriteLine("Neispravan unos, unesite a ili b!");
            DateOption = Console.ReadLine().ToLower();
        }

        if (DateOption == "a")
        {
            Date = DateTime.Now;
        }
        else if (DateOption == "b")
        {
            Console.WriteLine("Unesi datum transakcije: (YYYY.MM.DD ");
            Date = NewDate();
        }
        else
        {
            break;
        }
    }
    while (DateOption != "a" && DateOption != "b");

    Console.WriteLine("Unesi tip transakcje: (prihod/rashod)");
    Ttype = Console.ReadLine().ToLower();
    while (Ttype != "prihod" && Ttype != "rashod")
    {
        Console.WriteLine("Neispravan tip, unesi ponovno: (prihod/rashod)");
        Ttype = Console.ReadLine().ToLower();
    }

    Console.WriteLine("Unesi iznos transakcje:");
    float.TryParse(Console.ReadLine(), out Amount);

    if (Ttype == "rashod" && Account.ToLower() == "current")
    {
        while (Amount <= 0 || Amount > AmountOnAcc)
        {
            Console.WriteLine("Neispravan iznos: (negativan iznos ili nedovoljan iznos na racunu");
            float.TryParse(Console.ReadLine(), out Amount);
        }
    }
    else
    {
        while (Amount <= 0)
        {
            Console.WriteLine("Neispravan iznos: (negativan iznos:");
            float.TryParse(Console.ReadLine(), out Amount);
        }
    }

    Console.WriteLine("Unesi opis: ");
    Description = Console.ReadLine();

    if (Description == "")
    {
        Description = "Standardna transakcija";
    }


    Console.WriteLine("Unesi kategoriju: ");
    int Odabir = -1;
    string[] CategoryOptions = CategoryRashodi;
    if (Ttype == "prihod")
    {
        CategoryOptions = CategoryPrihodi;
    }

    do
    {
        int counter = 1;
        foreach (var Element in CategoryOptions)
        {
            Console.WriteLine($"{counter} - {Element}");
            counter++;
        }

        int.TryParse(Console.ReadLine(), out Odabir);
    }
    while (Odabir < 1 || Odabir > CategoryOptions.Length);

    Category = CategoryOptions[Odabir - 1];

    if (Ttype.ToLower() == "rashod")
    {
        Amount = -Amount;
    }

    UserDictionary[UserID].TransactionLog[Guid.NewGuid().GetHashCode()] = (Ttype, Amount, Description, Category, Date, Account);


    var userData = UserDictionary[UserID];
    switch (Account.ToLower())
    {
        case "current":
            UserDictionary[UserID] = (userData.Name, userData.Surname, userData.Date, userData.Current + Amount, userData.Checking, userData.Prepaid, userData.TransactionLog);

            break;
        case "checking":
            UserDictionary[UserID] = (userData.Name, userData.Surname, userData.Date, userData.Current, userData.Checking + Amount, userData.Prepaid, userData.TransactionLog);

            break;
        case "prepaid":
            UserDictionary[UserID] = (userData.Name, userData.Surname, userData.Date, userData.Current, userData.Checking, userData.Prepaid + Amount, userData.TransactionLog);

            break;
        default:
            break;
    }
}

void DeleteTransction(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    string OverviewOption = "";
    do
    {
        Console.WriteLine("3 - Brisanje transakcije:");
        Console.WriteLine("a - po id-u");
        Console.WriteLine("b - ispod unesenog iznosa");
        Console.WriteLine("c - iznad unesenog iznosa");
        Console.WriteLine("d - svih prihoda");
        Console.WriteLine("e - svih rashoda");
        Console.WriteLine("f - svih transakcija za odabranu kategoriju");
        Console.WriteLine("g - izlaz");

        OverviewOption = Console.ReadLine();
        OverviewOption = OverviewOption.ToLower();

        var ValidOptions = new string[] { "a", "b", "c", "d", "e", "f", "g" };

        while (!ValidOptions.Contains(OverviewOption))
        {
            Console.WriteLine("Neispravan unos, unesite ponovno:");
            OverviewOption = Console.ReadLine().ToLower();
        }

        switch (OverviewOption)
        {
            case "a":
                DeleteTransctionID(TransactionLog, Account);
                break;
            case "b":
                DeleteTransctionAmount(TransactionLog, Account, "ispod");
                break;
            case "c":
                DeleteTransctionAmount(TransactionLog, Account, "iznad");
                break;
            case "d":
                DeleteTransctionTtype(TransactionLog, Account, "prihod");
                break;
            case "e":
                DeleteTransctionTtype(TransactionLog, Account, "rashod");
                break;
            case "f":
                DeleteTransctionCategory(TransactionLog, Account);
                break;
            default:
                break;
        }

    }
    while (OverviewOption != "g");
}

void EditTransaction(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    string[] CategoryPrihodi = { "plaća", "honorar", "poklon", "dividende", "bonusi" };
    string[] CategoryOptions = { "hrana", "prijevoz", "sport", "stanarina", "zdravstvo" };

    int TargetTransactionId;
    string NewDescription = "";
    string NewCategory = "";
    DateTime NewDatee;
    string stringYN;

    Console.WriteLine("Unesi ID transakcije koju zelite urediti:");
    int.TryParse(Console.ReadLine(), out TargetTransactionId);

    if (!TransactionLog.ContainsKey(TargetTransactionId))
    {
        Console.WriteLine($"Korisni sa ID {TargetTransactionId} ne postoji");
        return;
    }

    float NewAmount = TransactionLog[TargetTransactionId].Amount;
    string NewTtype = TransactionLog[TargetTransactionId].Ttype;

    Console.WriteLine($"Zelite urediti transakciju: {TargetTransactionId} - {TransactionLog[TargetTransactionId].Ttype} - {TransactionLog[TargetTransactionId].Amount} - {TransactionLog[TargetTransactionId].Description} - {TransactionLog[TargetTransactionId].Category} - {TransactionLog[TargetTransactionId].Date.ToString("yyyy-MM-dd")}? Y/N");
    stringYN = Console.ReadLine().ToLower();

    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine().ToLower();
    }
    if (stringYN == "n")
    {
        return;
    }
    stringYN = "";

    Console.WriteLine("Zelite li urediti opis? Y/N");
    stringYN = Console.ReadLine().ToLower();

    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine().ToLower();
    }
    if (stringYN == "y")
    {
        Console.WriteLine("Unesite novo ime:");
        NewDescription = Console.ReadLine();
        while (NewDescription == "" || NewDescription == " ")
        {
            NewDescription = "Standardna Transakcija";
        }
    }
    else
    {
        NewDescription = TransactionLog[TargetTransactionId].Description;
    }
    stringYN = "";

    int Odabir = -1;
    Console.WriteLine("Zelite li urediti kategoriju? Y/N");
    stringYN = Console.ReadLine().ToLower();

    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine().ToLower();
    }
    if (stringYN == "y")
    {
        Console.WriteLine("Unesi kategoriju: ");

        if (TransactionLog[TargetTransactionId].Category == "prihod")
        {
            CategoryOptions = CategoryPrihodi;
        }

        do
        {
            int counter = 1;
            foreach (var Element in CategoryOptions)
            {
                Console.WriteLine($"{counter} - {Element}");
                counter++;
            }

            int.TryParse(Console.ReadLine(), out Odabir);
        }
        while (Odabir < 1 || Odabir > CategoryOptions.Length);

        NewCategory = CategoryOptions[Odabir - 1];
    }
    else
    {
        NewCategory = TransactionLog[TargetTransactionId].Category;
    }
    stringYN = "";

    Console.WriteLine("Zelite li urediti datum ? Y/N");
    stringYN = Console.ReadLine().ToLower();

    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine().ToLower();
    }
    if (stringYN == "y")
    {
        Console.WriteLine("Unesite novi datum: (YYYY.MM.DD");
        NewDatee = NewDate();
    }
    else
    {
        NewDatee = TransactionLog[TargetTransactionId].Date;
    }
    stringYN = "";

    Console.WriteLine($"Sigurni ste da zelite promjeniti podatke u: {TargetTransactionId}-{NewTtype}-{NewAmount}-{NewDescription}-{NewCategory}-{NewDatee.ToString("yyyy-MM-dd")}-{Account}? Y/N");
    stringYN = Console.ReadLine();
    stringYN = stringYN.ToLower();
    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine();
        stringYN = stringYN.ToLower();
    }
    if (stringYN == "y")
    {

        var user = TransactionLog[TargetTransactionId];

        TransactionLog[TargetTransactionId] = (
            Ttype: NewTtype,
            Amount: NewAmount,
            Description: NewDescription,
            Category: NewCategory,
            Date: NewDatee,
            Account: Account
        );
        Console.WriteLine("Podatci uspjesno promjenjeni.");
    }
}

void DeleteTransctionID(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    int DeleteTransactionID;
    Console.WriteLine("Unesi ID transakcije koju zelite izbrisati:");
    int.TryParse(Console.ReadLine(), out DeleteTransactionID);

    if (TransactionLog.ContainsKey(DeleteTransactionID))
    {
        Console.WriteLine($"Sigurni ste da zelite izbrisati transakciju: {DeleteTransactionID}-{TransactionLog[DeleteTransactionID].Ttype}-{TransactionLog[DeleteTransactionID].Amount}-{TransactionLog[DeleteTransactionID].Description}-{TransactionLog[DeleteTransactionID].Category}-{TransactionLog[DeleteTransactionID].Date.ToString("yyyy-MM-dd")}? Y/N");
        string stringYN = Console.ReadLine().ToLower();
        while (stringYN != "y" && stringYN != "n")
        {
            stringYN = Console.ReadLine();
            stringYN = stringYN.ToLower();
        }
        if (stringYN == "y")
        {
            if (TransactionLog.Remove(DeleteTransactionID))
            {
                Console.WriteLine($"Tranaskcija sa ID {DeleteTransactionID} je izbrisana.");
            }
        }
        else
        {
            Console.WriteLine($"Ponisteno!");
        }
    }
    else
    {
        Console.WriteLine($"Nepostoji odabrani transakcija.");
    }

}
void DeleteTransctionAmount(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account, string Filter)
{
    int DeleteAmount;
    Console.WriteLine("Unesi ID korisnika kojeg zelite izbrisati:");
    int.TryParse(Console.ReadLine(), out DeleteAmount);

    while (DeleteAmount <= 0)
    {
        Console.WriteLine("Neispravan unos iznosa:");
        int.TryParse(Console.ReadLine(), out DeleteAmount);
    }

    foreach (var element in TransactionLog)
    {
        if (element.Value.Account == Account)
        {
            if (Filter == "ispod")
            {
                if (element.Value.Amount < DeleteAmount)
                {
                    Console.WriteLine($"{element.Key}-{element.Value.Ttype}-{element.Value.Amount}-{element.Value.Description}-{element.Value.Category}-{element.Value.Date.ToString("yyyy-MM-dd")}");

                }
            }
            else
            {
                if (element.Value.Amount > DeleteAmount)
                {
                    Console.WriteLine($"{element.Key}-{element.Value.Ttype}-{element.Value.Amount}-{element.Value.Description}-{element.Value.Category}-{element.Value.Date.ToString("yyyy-MM-dd")}");

                }
            }
        }
    }
    Console.WriteLine($"Sigurni ste da zelite izbrisati ove transakcije: (Y/N)");
    string stringYN = Console.ReadLine().ToLower();
    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine();
        stringYN = stringYN.ToLower();
    }
    if (stringYN == "y")
    {
        foreach (var element in TransactionLog)
        {
            if (element.Value.Account == Account)
            {
                if (Filter == "ispod")
                {
                    if (element.Value.Amount < DeleteAmount)
                    {
                        TransactionLog.Remove(element.Key);

                    }
                }
                else
                {
                    if (element.Value.Amount > DeleteAmount)
                    {
                        TransactionLog.Remove(element.Key);

                    }
                }
            }
        }
        Console.WriteLine("Uspjesno obrisane");
    }
    else
    {
        Console.WriteLine("ponisteno");
    }
}
void DeleteTransctionTtype(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account, string Ttype)
{
    foreach (var element in TransactionLog)
    {
        if (element.Value.Ttype == Ttype && element.Value.Account == Account)
        {
            Console.WriteLine($"{element.Key}-{element.Value.Ttype}-{element.Value.Amount}-{element.Value.Description}-{element.Value.Category}-{element.Value.Date.ToString("yyyy-MM-dd")}");
        }
    }
    Console.WriteLine($"Sigurni ste da zelite izbrisati ove transakcije: (Y/N)");
    string stringYN = Console.ReadLine().ToLower();
    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine();
        stringYN = stringYN.ToLower();
    }
    if (stringYN == "y")
    {
        foreach (var element in TransactionLog)
        {
            if (element.Value.Ttype == Ttype && element.Value.Account == Account)
            {
                TransactionLog.Remove(element.Key);
            }
        }
        Console.WriteLine("Uspjesno izbrisane");
    }
    else
    {
        Console.WriteLine("Ponisteno");
    }
}

void DeleteTransctionCategory(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    string[] CategoryOptions = { "plaća", "honorar", "poklon", "dividende", "bonusi", "hrana", "prijevoz", "sport", "stanarina", "zdravstvo" };

    Console.WriteLine("Odaberi kategoriju: ");
    int Odabir = -1;
    string Category = "";
    do
    {
        int counter = 1;
        foreach (var Element in CategoryOptions)
        {
            Console.WriteLine($"{counter} - {Element}");
            counter++;
        }

        int.TryParse(Console.ReadLine(), out Odabir);
    }
    while (Odabir < 1 || Odabir > CategoryOptions.Length);

    Category = CategoryOptions[Odabir - 1];

    foreach (var element in TransactionLog)
    {
        if (element.Value.Account == Account && element.Value.Category == Category)
        {
            Console.WriteLine($"{element.Key}-{element.Value.Ttype}-{element.Value.Amount}-{element.Value.Description}-{element.Value.Category}-{element.Value.Date.ToString("yyyy-MM-dd")}");
        }
    }
    Console.WriteLine($"Sigurni ste da zelite izbrisati ove transakcije: (Y/N)");
    string stringYN = Console.ReadLine().ToLower();
    while (stringYN != "y" && stringYN != "n")
    {
        stringYN = Console.ReadLine();
        stringYN = stringYN.ToLower();
    }
    if (stringYN == "y")
    {
        foreach (var element in TransactionLog)
        {
            if (element.Value.Account == Account && element.Value.Category == Category)
            {
                TransactionLog.Remove(element.Key);
            }
        }
        Console.WriteLine("Uspjesno izbrisane");
    }
    else
    {
        Console.WriteLine("Ponisteno");
    }
}

void TranscationOverview(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    string OverviewOption = "";
    do
    {
        Console.WriteLine("4 - Pregled transakcija:");
        Console.WriteLine("a - sve transakcije kako su spremljene");
        Console.WriteLine("b - sve transakcije sortirane po iznosu uzlazno");
        Console.WriteLine("c - sve transakcije sortirane po iznosu silazno");
        Console.WriteLine("d - sve transakcije sortirane po opisu abecedno");
        Console.WriteLine("e - sve transakcije sortirane po datumu uzlazno");
        Console.WriteLine("f - sve transakcije sortirane po datumu silazno");
        Console.WriteLine("g - svi prihodi");
        Console.WriteLine("h - svi rashodi");
        Console.WriteLine("i - sve transakcije za odabranu kategoriju");
        Console.WriteLine("j - sve transakcije za odabrani tip i kategoriju");
        Console.WriteLine("k - izlaz");

        OverviewOption = Console.ReadLine();
        OverviewOption = OverviewOption.ToLower();

        var ValidOptions = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k" };

        while (!ValidOptions.Contains(OverviewOption))
        {
            Console.WriteLine("Neispravan unos, unesite ponovno:");
            OverviewOption = Console.ReadLine().ToLower();
        }

        switch (OverviewOption)
        {
            case "a":
                PrintAllTransaction(TransactionLog, Account);
                break;
            case "b":
                PrintAmountSortedTransaction(TransactionLog, Account, "U");
                break;
            case "c":
                PrintAmountSortedTransaction(TransactionLog, Account, "S");
                break;
            case "d":
                PrintDescriptionSortedTransaction(TransactionLog, Account);
                break;
            case "e":
                PrintDateSortedTransaction(TransactionLog, Account, "U");
                break;
            case "f":
                PrintDateSortedTransaction(TransactionLog, Account, "S");
                break;
            case "g":
                PrintTransactionTtype(TransactionLog, Account, "prihod");
                break;
            case "h":
                PrintTransactionTtype(TransactionLog, Account, "rashod");
                break;
            case "i":
                PrintTransactionCategory(TransactionLog, Account);
                break;
            case "j":
                PrintTransactionCategoryTtype(TransactionLog, Account);
                break;


            default:
                break;
        }

    }
    while (OverviewOption != "k");
}

void PrintAllTransaction(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    Console.WriteLine("ID - Tip - Iznos - Opis - Kategorija - Datum");
    foreach (var Element in TransactionLog)
    {
        if (Element.Value.Account == Account)
        {
            Console.WriteLine($"{Element.Key} - {Element.Value.Ttype} - {Element.Value.Amount} - {Element.Value.Description} - {Element.Value.Category} - {Element.Value.Date.ToString("yyyy-MM-dd")}");
        }
    }
}

void PrintAmountSortedTransaction(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account, string Decending)
{
    var SortedTransaction = TransactionLog
            .OrderBy(entry => entry.Value.Amount)
            .ToList();

    if (Decending == "S")
    {
        SortedTransaction = TransactionLog
            .OrderBy(entry => entry.Value.Amount).Reverse()
            .ToList();
        Console.WriteLine("Ispis svih transakcija sortirane po iznosu silazno");

    }
    else
    {
        Console.WriteLine("Ispis svih transakcija sortirane po iznosu uzlazno");
    }

    Console.WriteLine("ID - Tip - Iznos - Opis - Kategorija - Datum");
    foreach (var Element in SortedTransaction)
    {
        if (Element.Value.Account == Account)
        {
            Console.WriteLine($"{Element.Key} - {Element.Value.Ttype} - {Element.Value.Amount} - {Element.Value.Description} - {Element.Value.Category} - {Element.Value.Date.ToString("yyyy-MM-dd")}");
        }
    }
}

void PrintDescriptionSortedTransaction(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    var SortedTransaction = TransactionLog
            .OrderBy(entry => entry.Value.Description)
            .ToList();

    Console.WriteLine("ID - Tip - Iznos - Opis - Kategorija - Datum");
    foreach (var Element in SortedTransaction)
    {
        if (Element.Value.Account == Account)
        {
            Console.WriteLine($"{Element.Key} - {Element.Value.Ttype} - {Element.Value.Amount} - {Element.Value.Description} - {Element.Value.Category} - {Element.Value.Date.ToString("yyyy-MM-dd")}");
        }
    }
}

void PrintDateSortedTransaction(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account, string Decending)
{
    var SortedTransaction = TransactionLog
            .OrderBy(entry => entry.Value.Date)
            .ToList();

    if (Decending == "S")
    {
        SortedTransaction = TransactionLog
            .OrderBy(entry => entry.Value.Date).Reverse()
            .ToList();
        Console.WriteLine("Ispis svih transakcija sortirane po datumu silazno");

    }
    else
    {
        Console.WriteLine("Ispis svih transakcija sortirane po datumu uzlazno");
    }

    Console.WriteLine("ID - Tip - Iznos - Opis - Kategorija - Datum");
    foreach (var Element in SortedTransaction)
    {
        if (Element.Value.Account == Account)
        {
            Console.WriteLine($"{Element.Key} - {Element.Value.Ttype} - {Element.Value.Amount} - {Element.Value.Description} - {Element.Value.Category} - {Element.Value.Date.ToString("yyyy-MM-dd")}");
        }
    }
}

void PrintTransactionTtype(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account, string Ttype)
{
    Console.WriteLine("ID - Tip - Iznos - Opis - Kategorija - Datum");
    foreach (var Element in TransactionLog)
    {
        if (Element.Value.Account == Account && Element.Value.Ttype == Ttype)
        {
            Console.WriteLine($"{Element.Key} - {Element.Value.Ttype} - {Element.Value.Amount} - {Element.Value.Description} - {Element.Value.Category} - {Element.Value.Date.ToString("yyyy-MM-dd")}");
        }
    }
}

void PrintTransactionCategory(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    string[] CategoryPrihodi = { "plaća", "honorar", "poklon", "dividende", "bonusi" };
    string[] CategoryRashodi = { "hrana", "prijevoz", "sport", "stanarina", "zdravstvo" };

    Console.WriteLine("Odaberi kategoriju: ");
    int Odabir = -1;
    string[] CategoryOptions = CategoryRashodi.Concat(CategoryPrihodi).ToArray();
    string Category = "";
    do
    {
        int counter = 1;
        foreach (var Element in CategoryOptions)
        {
            Console.WriteLine($"{counter} - {Element}");
            counter++;
        }

        int.TryParse(Console.ReadLine(), out Odabir);
    }
    while (Odabir < 1 || Odabir > CategoryOptions.Length);

    Category = CategoryOptions[Odabir - 1];


    Console.WriteLine("ID - Tip - Iznos - Opis - Kategorija - Datum");
    foreach (var Element in TransactionLog)
    {
        if (Element.Value.Account == Account && Element.Value.Category == Category)
        {
            Console.WriteLine($"{Element.Key} - {Element.Value.Ttype} - {Element.Value.Amount} - {Element.Value.Description} - {Element.Value.Category} - {Element.Value.Date.ToString("yyyy-MM-dd")}");
        }
    }
}

void PrintTransactionCategoryTtype(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    string[] CategoryPrihodi = { "plaća", "honorar", "poklon", "dividende", "bonusi" };
    string[] CategoryRashodi = { "hrana", "prijevoz", "sport", "stanarina", "zdravstvo" };

    string Ttype = "";

    Console.WriteLine("Unesi tip transakcje: (prihod/rashod)");
    Ttype = Console.ReadLine().ToLower();
    while (Ttype != "prihod" && Ttype != "rashod")
    {
        Console.WriteLine("Neispravan tip, unesi ponovno: (prihod/rashod)");
        Ttype = Console.ReadLine().ToLower();
    }


    Console.WriteLine("Odaberi kategoriju: ");
    int Odabir = -1;
    string[] CategoryOptions = CategoryRashodi;
    string Category = "";

    if (Ttype == "prhodi")
    {
        CategoryOptions = CategoryPrihodi;
    }
    do
    {
        int counter = 1;
        foreach (var Element in CategoryOptions)
        {
            Console.WriteLine($"{counter} - {Element}");
            counter++;
        }

        int.TryParse(Console.ReadLine(), out Odabir);
    }
    while (Odabir < 1 || Odabir > CategoryOptions.Length);

    Category = CategoryOptions[Odabir - 1];


    Console.WriteLine("ID - Tip - Iznos - Opis - Kategorija - Datum");
    foreach (var Element in TransactionLog)
    {
        if (Element.Value.Account == Account && Element.Value.Category == Category)
        {
            Console.WriteLine($"{Element.Key} - {Element.Value.Ttype} - {Element.Value.Amount} - {Element.Value.Description} - {Element.Value.Category} - {Element.Value.Date.ToString("yyyy-MM-dd")}");
        }
    }
}

void FinancialReport(Dictionary<int, (string Name, string Surname, DateTime Date, float Current, float Checking, float Prepaid, Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog)> UserDictionary, int UserID, string Account)
{
    string OverviewOption = "";
    do
    {
        Console.WriteLine("5 - Finacncijsko izvjesce:");
        Console.WriteLine("a - trenutno stanje računa");
        Console.WriteLine("b - broj ukupnih transakcija");
        Console.WriteLine("c - ukupan iznos prihoda i rashoda za odabrani mjesec i godinu");
        Console.WriteLine("d - postotak udjela rashoda za odabranu kategoriju");
        Console.WriteLine("e - izlaz");

        OverviewOption = Console.ReadLine();
        OverviewOption = OverviewOption.ToLower();

        var ValidOptions = new string[] { "a", "b", "c", "d", "e" };

        while (!ValidOptions.Contains(OverviewOption))
        {
            Console.WriteLine("Neispravan unos, unesite ponovno:");
            OverviewOption = Console.ReadLine().ToLower();
        }

        switch (OverviewOption)
        {
            case "a":
                float AmountOnAcc = 0;
                switch (Account.ToLower())
                {
                    case "current":
                        AmountOnAcc = UserDictionary[UserID].Current;
                        break;
                    case "checking":
                        AmountOnAcc = UserDictionary[UserID].Checking;
                        break;
                    case "prepaid":
                        AmountOnAcc = UserDictionary[UserID].Prepaid;
                        break;
                    default:
                        break;
                }
                Console.WriteLine($"Trenutno stanje na racunu je: {AmountOnAcc}");
                if (AmountOnAcc < 0)
                {
                    Console.WriteLine("Upozorenje, racun u minusu");
                }
                break;
            case "b":
                Console.WriteLine($"Ukupan broj tranaskcija je: {NumberOfTransactions(UserDictionary[UserID].TransactionLog, Account)}");
                break;
            case "c":
                MonthFinancialReport(UserDictionary[UserID].TransactionLog, Account);
                break;
            case "d":
                RashodInCategory(UserDictionary[UserID].TransactionLog, Account);
                break;
            default:
                break;
        }

    }
    while (OverviewOption != "e");
}

int NumberOfTransactions(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    int Number = 0;
    foreach (var element in TransactionLog)
    {
        if (element.Value.Account == Account)
        {
            Number++;
        }
    }
    return Number;
}

void MonthFinancialReport(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{

    DateTime DateStart;
    DateTime DateEnd;
    Console.WriteLine("Unesi od kojeg datuma zelis ipis: (YYYY-MM)");
    bool correct = DateTime.TryParse(Console.ReadLine(), out DateStart);
    while (!correct)
    {
        Console.WriteLine("Neispavan unos datuma, pokušajte ponovo: (YYYY-MM)");
        correct = DateTime.TryParse(Console.ReadLine(), out DateStart);

    }
    Console.WriteLine("Unesi do kojeg datuma zelis ipis:");
    correct = DateTime.TryParse(Console.ReadLine(), out DateEnd);
    while (!correct)
    {
        Console.WriteLine("Neispavan unos datuma, pokušajte ponovo: (YYYY-MM)");
        correct = DateTime.TryParse(Console.ReadLine(), out DateEnd);

    }
    Console.WriteLine("ID - Tip - Iznos - Opis - Kategorija - Datum");
    float prihodi = 0;
    float rashodi = 0;
    foreach (var Element in TransactionLog)
    {
        if (Element.Value.Account == Account && Element.Value.Date >= DateStart && Element.Value.Date <= DateEnd)
        {
            if (Element.Value.Ttype == "rashod")
            {
                rashodi -= Element.Value.Amount;
            }
            else
            {
                prihodi += Element.Value.Amount;
            }
        }
    }
    Console.WriteLine($"za razdoblje od {DateStart.ToString("yyyy-MM")} od {DateEnd.ToString("yyyy-MM")} prihodi iznose: {prihodi}, a rashodi {rashodi}");

}

void RashodInCategory(Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog, string Account)
{
    string[] CategoryOptions = { "hrana", "prijevoz", "sport", "stanarina", "zdravstvo" };
    int Odabir = 0;
    do
    {
        int counter = 1;
        foreach (var Element in CategoryOptions)
        {
            Console.WriteLine($"{counter} - {Element}");
            counter++;
        }

        int.TryParse(Console.ReadLine(), out Odabir);
    }
    while (Odabir < 1 || Odabir > CategoryOptions.Length);

    int CounterRashodi = 0;
    int CategoryCounter = 0;

    foreach (var element in TransactionLog)
    {
        if (element.Value.Account == Account && element.Value.Ttype == "rashod")
        {
            CounterRashodi++;
            if (element.Value.Category == CategoryOptions[Odabir - 1])
            {
                CategoryCounter++;
            }
        }
    }
    if(CounterRashodi == 0)
    {
        Console.WriteLine($"ne postoji rashod sa kategorijom {CategoryOptions[Odabir - 1]}");
    }
    else
    {
        Console.WriteLine($"Posototak rashoda sa kategorijom {CategoryOptions[Odabir - 1]} je : {CategoryCounter / CounterRashodi}");
    }



}


string[] CategoryPrihodi = { "plaća", "honorar", "poklon", "dividende", "bonusi" };
string[] CategoryRashodi = { "hrana", "prijevoz", "sport", "stanarina", "zdravstvo" };

int input = -1;
var UserDictionary = new Dictionary<int, (
    string Name,
    string Surname,
    DateTime Date,
    float Current,
    float Checking,
    float Prepaid,
    Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)> TransactionLog
)>
{
    { 100, (
        "Ante",
        "matic",
        DateTime.Parse("2003-10-05"),
        100,
        0,
        0,
        new Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)>
        {
            { 1, ("prihod", 100, "plaća", "plaća", DateTime.Parse("2024-10-05"), "current") },
            { 2, ("rashod", -50, "hrana", "hrana", DateTime.Parse("2024-11-05"), "prepaid") },
            { 3, ("prihod", 200, "honorar", "honorar", DateTime.Parse("2024-09-05"), "prepaid") },
            { 4, ("rashod", -60, "prijevoz", "prijevoz", DateTime.Parse("2023-09-05"), "prepaid") },
            { 5, ("prihod", 150, "poklon", "poklon", DateTime.Parse("2024-08-15"), "checking") },
            { 6, ("rashod", -30, "sport", "sport", DateTime.Parse("2024-07-05"), "current") },
            { 7, ("prihod", 300, "dividende", "dividende", DateTime.Parse("2024-06-05"), "current") },
            { 8, ("rashod", -40, "stanarina", "stanarina", DateTime.Parse("2024-05-05"), "prepaid") },
            { 9, ("prihod", 120, "bonusi", "bonusi", DateTime.Parse("2024-04-05"), "checking") },
            { 10, ("rashod", -70, "zdravstvo", "zdravstvo", DateTime.Parse("2024-03-05"), "current") },
            { 11, ("prihod", 250, "plaća", "plaća", DateTime.Parse("2024-02-05"), "current") },
            { 12, ("rashod", -90, "stanarina", "stanarina", DateTime.Parse("2024-01-05"), "checking") },
            { 13, ("prihod", 400, "honorar", "honorar", DateTime.Parse("2023-12-05"), "prepaid") },
            { 14, ("rashod", -55, "hrana", "hrana", DateTime.Parse("2023-11-05"), "current") },
            { 15, ("prihod", 500, "bonusi", "bonusi", DateTime.Parse("2023-10-05"), "checking") },
            { 16, ("prihod", 100, "plaća", "plaća", DateTime.Parse("2024-10-05"), "current") },
            { 17, ("rashod", -50, "hrana", "hrana", DateTime.Parse("2024-11-05"), "prepaid") },
            { 18, ("prihod", 2000, "honorar", "honorar", DateTime.Parse("2020-09-05"), "prepaid") },
            { 19, ("rashod", -60, "prijevoz", "prijevoz", DateTime.Parse("2023-09-05"), "prepaid") },
            { 20, ("prihod", 150, "poklon", "poklon", DateTime.Parse("2024-08-15"), "checking") },
            { 21, ("rashod", -30, "sport", "sport", DateTime.Parse("2024-07-05"), "current") },
            { 22, ("prihod", 300, "dividende", "dividende", DateTime.Parse("2024-06-05"), "current") },
            { 23, ("rashod", -40, "stanarina", "stanarina", DateTime.Parse("2024-05-05"), "prepaid") },
            { 24, ("prihod", 120, "bonusi", "bonusi", DateTime.Parse("2024-04-05"), "checking") },
            { 25, ("rashod", -70, "zdravstvo", "zdravstvo", DateTime.Parse("2024-03-05"), "current") },
            { 26, ("prihod", 250, "plaća", "plaća", DateTime.Parse("2024-02-05"), "current") },
            { 27, ("rashod", -90, "stanarina", "stanarina", DateTime.Parse("2024-01-05"), "checking") },
            { 28, ("prihod", 400, "honorar", "honorar", DateTime.Parse("2023-12-05"), "prepaid") },
            { 29, ("rashod", -55, "hrana", "hrana", DateTime.Parse("2023-11-05"), "current") },
            { 30, ("prihod", 500, "bonusi", "bonusi", DateTime.Parse("2023-10-05"), "checking") }

        }
    )},
    { "Bob".GetHashCode(), (
        "Bob",
        "Johnson",
        DateTime.Parse("2000-10-05"),
        1500,
        500,
        700,
        new Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)>
        {
            { 1, ("Payment", 100, "Monthly fee", "Subscription", DateTime.Parse("2024-10-05"), "current") },
            { 2, ("Refund", -50, "Service refund", "Finance", DateTime.Parse("2022-10-05"), "prepaid") }
        }
    )},
    { "Charli".GetHashCode(), (
        "Charlie",
        "Brown",
        DateTime.Parse("1994-10-05"),
        1200,
        200,
        400,
        new Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)>
        {
            { 1, ("Payment", 100, "Monthly fee", "Subscription", DateTime.Parse("2024-01-05"), "current") },
            { 2, ("Refund", -50, "Service refund", "Finance", DateTime.Parse("2024-10-15"), "prepaid") }
        }
    )},
    { "David".GetHashCode(), (
        "David",
        "Williams",
        DateTime.Parse("1990-10-05"),
        3000,
        1500,
        1000,
        new Dictionary<int, (string Ttype, float Amount, string Description, string Category, DateTime Date, string Account)>
        {
            { 1, ("Payment", 100, "Monthly fee", "Subscription", DateTime.Parse("2024-05-05"), "current") },
            { 2, ("Refund", -50, "Service refund", "Finance", DateTime.Parse("2024-10-25"), "prepaid") }
        }
    )}
};


do
{
    Console.WriteLine("\n");
    Console.WriteLine("1 - Korisnici");
    Console.WriteLine("2 - Racuni");
    Console.WriteLine("3 - Izlaz iz aplikacije");

    int.TryParse(Console.ReadLine(), out input);

    if (input == 3)
    {
        break;
    }
    else if (input == 1)
    {
        UserFunction(UserDictionary);
    }
    else if (input == 2)
    {
        AccountFunction(UserDictionary);
    }
    else
    {
        Console.WriteLine("Nesipravan unos pokušaj ponovo.");
    }

}
while (input != 3);