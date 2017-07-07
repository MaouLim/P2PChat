/* 
 * created by Maou Lim on 17-4-10
 */

namespace P2PChat {
    public interface UIComponent {

        /* a ui must be able to display the infomation */
        void Display(string info, bool err);

        /* a ui must be able to submit the user's input to the background */
        string Input();
    }

    /* a console keyboard */
    public class KeyBoard : UIComponent {

        public void Display(string info, bool err) {
            if (err) {
                System.Console.Error.Write(info);
            }
            else {
                System.Console.Write(info);
            }
        }

        public string Input() {
            return System.Console.ReadLine();
        }
    }

}