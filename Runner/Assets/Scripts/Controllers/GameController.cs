using Assets.Scripts.Events;
using Assets.Scripts.Menus;
using UnityEngine;
using Event = Assets.Scripts.Events.Event;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private Generator _generator;
        [SerializeField] private CameraController _camera;
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private GameMenu _gameMenu;
        [SerializeField] private GameOverMenu _gameOverMenu;
        [SerializeField] private HudController _hud;

        void Start()
        {
            _generator.Setup();
            _hud.Setup(_player.gameObject);

            SetupMenus();

            EventController.Subscribe(
                new EventSubscription(GameOver, typeof(GameEndEvent).ToString()));
        }

        private void SetupMenus()
        {
            _gameMenu.Setup(BackToMainMenuFromGame);
            _mainMenu.Setup(StartGame);
            _gameOverMenu.Setup(BackToMainMenuFromGameOver, StartGame);
            
            _mainMenu.Open();
            _gameMenu.Close();
            _gameOverMenu.Close();
        }
        
        private void StartGame()
        {
            _generator.StartGame();
            _player.StartGame();
            _camera.StartGame();
            _hud.StartGame();
            
            _mainMenu.Close();
            _gameOverMenu.Close();
            _gameMenu.Open();
        }

        private void GameOver(Event gameEndEvent)
        {
            _gameMenu.Close();
            _camera.EndGame(true, () =>
            {
                _gameMenu.Close();
                _generator.EndGame();
                _generator.PrepareGame();
                _hud.EndGame();
                _gameOverMenu.Open();
            });
        }


        private void BackToMainMenuFromGameOver()
        {
            _mainMenu.Open();
        }
        
        private void BackToMainMenuFromGame()
        {
            _player.EndGame();
            _camera.EndGame(false);
            _generator.EndGame();
            _generator.PrepareGame();
            _mainMenu.Open();
        }
    }
}