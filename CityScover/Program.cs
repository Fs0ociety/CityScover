//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 09/10/2018
//

using CityScover.Services;
using System.Threading.Tasks;
using static System.Console;

namespace CityScover
{
   class Program
   {
      static async Task Main(string[] args)
      {
         DisplayLogo();
         ISolverService solverService = SolverService.Instance;
         await solverService.Run();
         WriteLine("Press any key to continue...");
         ReadKey();
      }

      #region Private static methods
      private static void DisplayLogo()
      {
         string logo = string.Empty;
         logo += "                                                                                                                   \n";
         logo += "                                                                                                                   \n";
         logo += "                 ██                                                                                                   \n";
         logo += "                                                                                                                   \n";
         logo += "      ▄████████  ▄█      ███     ▄██   ▄      ▄████████  ▄████████  ▄██████▄   ▄█    █▄     ▄████████    ▄████████ \n";
         logo += "     ███    ███ ███  ▀█████████▄ ███   ██▄   ███    ███ ███    ███ ███    ███ ███    ███   ███    ███   ███    ███ \n";
         logo += "     ███    █▀  ███▌    ▀███▀▀██ ███▄▄▄███   ███    █▀  ███    █▀  ███    ███ ███    ███   ███    █▀    ███    ███ \n";
         logo += "     ███        ███▌     ███   ▀ ▀▀▀▀▀▀███   ███        ███        ███    ███ ███    ███  ▄███▄▄▄      ▄███▄▄▄▄██▀ \n";
         logo += "     ███        ███▌     ███     ▄██   ███ ▀███████████ ███        ███    ███ ███    ███ ▀▀███▀▀▀     ▀▀███▀▀▀▀▀   \n";
         logo += "     ███    █▄  ███      ███     ███   ███          ███ ███    █▄  ███    ███ ███    ███   ███    █▄  ▀███████████ \n";
         logo += "     ███    ███ ███      ███     ███   ███    ▄█    ███ ███    ███ ███    ███ ███    ███   ███    ███   ███    ███ \n";
         logo += "     ████████▀  █▀      ▄████▀    ▀█████▀   ▄████████▀  ████████▀   ▀██████▀   ▀██████▀    ██████████   ███    ███ \n";
         logo += "                                                                                                        ███    ███ \n";
         logo += "                                                                                                                   \n";
         WriteLine($"{logo}");
      }

      private static void DisplayLogo2()
      {
         string logo = string.Empty;
         logo += "                                                                                                                                                                                                  \n";
         logo += "        CCCCCCCCCCCCC  iiii           tttt                                  SSSSSSSSSSSSSSS                                                                                                       \n";
         logo += "     CCC::::::::::::C i::::i       ttt:::t                                SS:::::::::::::::S                                                                                                      \n";
         logo += "   CC:::::::::::::::C  iiii       t:::::t                                S:::::SSSSSS::::::S                                                                                                      \n";
         logo += "  C:::::CCCCCCCC::::C             t:::::t                                S:::::S     SSSSSSS                                                                                                      \n";
         logo += " C:::::C       CCCCCCiiiiiiittttttt:::::tttttttyyyyyyy           yyyyyyy S:::::S                 cccccccccccccccc    ooooooooooo  vvvvvvv           vvvvvvv eeeeeeeeeeee     rrrrr   rrrrrrrrr    \n";
         logo += "C:::::C              i:::::it:::::::::::::::::t y:::::y         y:::::y  S:::::S               cc:::::::::::::::c  oo:::::::::::oo v:::::v         v:::::vee::::::::::::ee   r::::rrr:::::::::r   \n";
         logo += "C:::::C               i::::it:::::::::::::::::t  y:::::y       y:::::y    S::::SSSS           c:::::::::::::::::c o:::::::::::::::o v:::::v       v:::::ve::::::eeeee:::::ee r:::::::::::::::::r  \n";
         logo += "C:::::C               i::::itttttt:::::::tttttt   y:::::y     y:::::y      SS::::::SSSSS     c:::::::cccccc:::::c o:::::ooooo:::::o  v:::::v     v:::::ve::::::e     e:::::e rr::::::rrrrr::::::r \n";
         logo += "C:::::C               i::::i      t:::::t          y:::::y   y:::::y         SSS::::::::SS   c::::::c     ccccccc o::::o     o::::o   v:::::v   v:::::v e:::::::eeeee::::::e  r:::::r     r:::::r \n";
         logo += "C:::::C               i::::i      t:::::t           y:::::y y:::::y             SSSSSS::::S  c:::::c              o::::o     o::::o    v:::::v v:::::v  e:::::::::::::::::e   r:::::r     rrrrrrr \n";
         logo += "C:::::C               i::::i      t:::::t            y:::::y:::::y                   S:::::S c:::::c              o::::o     o::::o     v:::::v:::::v   e::::::eeeeeeeeeee    r:::::r             \n";
         logo += " C:::::C       CCCCCC i::::i      t:::::t    tttttt   y:::::::::y                    S:::::S c::::::c     ccccccc o::::o     o::::o      v:::::::::v    e:::::::e             r:::::r             \n";
         logo += "  C:::::CCCCCCCC::::Ci::::::i     t::::::tttt:::::t    y:::::::y        SSSSSSS      S:::::S c:::::::cccccc:::::c o:::::ooooo:::::o       v:::::::v     e::::::::e            r:::::r             \n";
         logo += "   CC:::::::::::::::Ci::::::i     tt::::::::::::::t     y:::::y         S::::::SSSSSS:::::S  c:::::::::::::::::c  o:::::::::::::::o        v:::::v       e::::::::eeeeeeee    r:::::r             \n";
         logo += "     CCC::::::::::::Ci::::::i       tt:::::::::::tt    y:::::y          S:::::::::::::::SS    cc:::::::::::::::c   oo:::::::::::oo          v:::v         ee:::::::::::::e    r:::::r             \n";
         logo += "        CCCCCCCCCCCCCiiiiiiii         ttttttttttt     y:::::y            SSSSSSSSSSSSSSS       cccccccccccccccc      ooooooooooo             vvv            eeeeeeeeeeeeee    rrrrrrr             \n";
         logo += "                                                     y:::::y                                                                                                                                      \n";
         logo += "                                                    y:::::y                                                                                                                                       \n";
         logo += "                                                   y:::::y                                                                                                                                        \n";
         logo += "                                                  y:::::y                                                                                                                                         \n";
         logo += "                                                 yyyyyyy                                                                                                                                          \n";
         logo += "                                                                                                                                                                                                  \n";
         WriteLine($"{logo}");
      }
      #endregion
   }
}
