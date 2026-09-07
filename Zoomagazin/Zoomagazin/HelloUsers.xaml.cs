using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Zoomagazin
{
    public partial class HelloUsers : Page
    {
        List<string> phrases = new List<string>()
        {
            "Ваша забота и нежность делают наших пушистых друзей счастливыми. Благодарим вас за то, что создаете уют и радость в их жизни.",
            "Ваши знания и опыт помогают нам находить идеальные дома для каждого пушистого малыша. Спасибо за ваше внимание и заботу!",
            "С каждым мяуканьем, гавканьем и мурлыканьем, мы знаем, что вы делаете свою работу с любовью. Благодарим вас за то, что помогаете нашим пушистым клиентам найти своих идеальных спутников.",
            "Ваша терпеливость и доброта помогают нам преодолевать все преграды и создавать гармонию между нашими пушистыми друзьями и их новыми семьями.",
            "Вы - наши ангелы-хранители пушистых созданий. Спасибо, что защищаете и бережно относитесь к каждому маленькому сердцу, которое оказывается в наших руках.",
            "Ваша любовь к животным превращает наш зоомагазин в сказочное место, где каждый четвероногий гость чувствует себя особенным. Благодарим вас за вашу преданность и заботу.",
            "Ваши улыбки и ласковые слова делают наш зоомагазин особенным для клиентов и пушистых обитателей. Спасибо за вашу доброту и душевность!",
            "Вы умеете разговаривать на языке пушистых и понимать их потребности. Благодарим вас за вашу интуицию и способность создавать гармоничные связи между людьми и животными.",
            "С каждым милым мурлыканьем и пушистым объятием, вы наполняете наши сердца радостью и счастьем. Благодарим вас за вашу преданность и любовь к пушистым созданиям."
        };

        int currentIndex = 0;

        public HelloUsers()
        {
            InitializeComponent();
            LikeShortsButton.Click += LikeShortsButton_Click;
        }

        private void LikeShortsButton_Click(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = Like;

            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            fadeOutAnimation.Completed += (s, args) =>
            {
                currentIndex = (currentIndex + 1) % phrases.Count;
                textBlock.Text = phrases[currentIndex];
                textBlock.BeginAnimation(OpacityProperty, fadeInAnimation);
            };

            textBlock.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }
    }
}