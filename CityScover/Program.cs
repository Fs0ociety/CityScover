//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 17/08/2018
//

using CityScover.Services;
using System.Configuration;
using static System.Console;

namespace CityScover
{
   class Program
   {
      static void Main(string[] args)
      {
         DisplayLogo();
         string configsPath = ConfigurationManager.AppSettings["ConfigsPath"];
         ISolverService solverService = SolverService.Instance;
         solverService.Run(configsPath);

         ReadKey();
      }

      #region Private static methods
      private static void DisplayLogo()
      {
         string logo = string.Empty;
         logo += "                                                                                                                                                                                                  ";
         logo += "        CCCCCCCCCCCCC  iiii           tttt                                  SSSSSSSSSSSSSSS                                                                                                       ";
         logo += "     CCC::::::::::::C i::::i       ttt:::t                                SS:::::::::::::::S                                                                                                      ";
         logo += "   CC:::::::::::::::C  iiii       t:::::t                                S:::::SSSSSS::::::S                                                                                                      ";
         logo += "  C:::::CCCCCCCC::::C             t:::::t                                S:::::S     SSSSSSS                                                                                                      ";
         logo += " C:::::C       CCCCCCiiiiiiittttttt:::::tttttttyyyyyyy           yyyyyyy S:::::S                 cccccccccccccccc    ooooooooooo  vvvvvvv           vvvvvvv eeeeeeeeeeee     rrrrr   rrrrrrrrr    ";
         logo += "C:::::C              i:::::it:::::::::::::::::t y:::::y         y:::::y  S:::::S               cc:::::::::::::::c  oo:::::::::::oo v:::::v         v:::::vee::::::::::::ee   r::::rrr:::::::::r   ";
         logo += "C:::::C               i::::it:::::::::::::::::t  y:::::y       y:::::y    S::::SSSS           c:::::::::::::::::c o:::::::::::::::o v:::::v       v:::::ve::::::eeeee:::::ee r:::::::::::::::::r  ";
         logo += "C:::::C               i::::itttttt:::::::tttttt   y:::::y     y:::::y      SS::::::SSSSS     c:::::::cccccc:::::c o:::::ooooo:::::o  v:::::v     v:::::ve::::::e     e:::::e rr::::::rrrrr::::::r ";
         logo += "C:::::C               i::::i      t:::::t          y:::::y   y:::::y         SSS::::::::SS   c::::::c     ccccccc o::::o     o::::o   v:::::v   v:::::v e:::::::eeeee::::::e  r:::::r     r:::::r ";
         logo += "C:::::C               i::::i      t:::::t           y:::::y y:::::y             SSSSSS::::S  c:::::c              o::::o     o::::o    v:::::v v:::::v  e:::::::::::::::::e   r:::::r     rrrrrrr ";
         logo += "C:::::C               i::::i      t:::::t            y:::::y:::::y                   S:::::S c:::::c              o::::o     o::::o     v:::::v:::::v   e::::::eeeeeeeeeee    r:::::r             ";
         logo += " C:::::C       CCCCCC i::::i      t:::::t    tttttt   y:::::::::y                    S:::::S c::::::c     ccccccc o::::o     o::::o      v:::::::::v    e:::::::e             r:::::r             ";
         logo += "  C:::::CCCCCCCC::::Ci::::::i     t::::::tttt:::::t    y:::::::y        SSSSSSS      S:::::S c:::::::cccccc:::::c o:::::ooooo:::::o       v:::::::v     e::::::::e            r:::::r             ";
         logo += "   CC:::::::::::::::Ci::::::i     tt::::::::::::::t     y:::::y         S::::::SSSSSS:::::S  c:::::::::::::::::c  o:::::::::::::::o        v:::::v       e::::::::eeeeeeee    r:::::r             ";
         logo += "     CCC::::::::::::Ci::::::i       tt:::::::::::tt    y:::::y          S:::::::::::::::SS    cc:::::::::::::::c   oo:::::::::::oo          v:::v         ee:::::::::::::e    r:::::r             ";
         logo += "        CCCCCCCCCCCCCiiiiiiii         ttttttttttt     y:::::y            SSSSSSSSSSSSSSS       cccccccccccccccc      ooooooooooo             vvv            eeeeeeeeeeeeee    rrrrrrr             ";
         logo += "                                                     y:::::y                                                                                                                                      ";
         logo += "                                                    y:::::y                                                                                                                                       ";
         logo += "                                                   y:::::y                                                                                                                                        ";
         logo += "                                                  y:::::y                                                                                                                                         ";
         logo += "                                                 yyyyyyy                                                                                                                                          ";
         logo += "                                                                                                                                                                                                  ";
         WriteLine($"{logo}");
      }
      #endregion
   }
}
