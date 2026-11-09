using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This script manages the player's health, money, score and fuel.
public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    // These variables store the player's health and the health slider settings.
    public int maxHealth = 100;
    public float Health = 100;
    public Slider HealthSlider;

    [Header("Other Stats")]
    // I use these variables to keep track of how much money the player has.
    public int Money;
    public int maxMoney;

    // This variable stores the player's current score.
    public int Score = 0;

    [Header("Fuel Settings")]
    // These variables control the player's fuel amount and fuel slider.
    public float Fuel = 10;
    public int maxFuel = 100;
    public Slider FuelSlider;

    [Header("Displays")]
    // I use these TextMeshPro variables to display the player's stats on the screen.
    public TextMeshProUGUI moneyDisplay;
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI healthDisplay;

    // Awake runs before Start and I use it to make sure the player's stats are within their limits.
    private void Awake()
    {
        // I use Mathf.Clamp to stop the player's health, money and fuel from going below zero or above their maximum values.
        Health = Mathf.Clamp(Health, 0, maxHealth);
        Money = Mathf.Clamp(Money, 0, maxMoney);
        Fuel = Mathf.Clamp(Fuel, 0, maxFuel);
    }

    // This function runs when the game starts.
    private void Start()
    {
        // I refresh the UI so that the player's stats are displayed correctly at the beginning.
        RefreshUI();
    }

    // Update runs every frame and keeps the player UI updated.
    private void Update()
    {
        UpdateUI();

    }

    // This function updates the sliders and text displays.
    private void UpdateUI()
    {
        // I update the health and fuel sliders using the player's current values.
        HealthSlider.value = Health;
        FuelSlider.value = Fuel;

        // I check if the fuel slider exists before updating its value.
        if (FuelSlider != null)
        {
            FuelSlider.value = Fuel;
        }

        // I call these functions to update the money and score text.
        UpdateMoneyDisplay();
        UpdateScoreDisplay();
    }

    // This function removes health from the player.
    public void hurt(float amount)
    {
        // I subtract the damage amount from the player's health.
        Health -= amount;

        // I make sure the health stays between zero and the maximum health.
        Health = Mathf.Clamp(Health, 0, maxHealth);

        // I update the UI to show the new health value.
        UpdateUI();
    }

    // This function restores health to the player.
    public void heal(int amount)
    {
        // I add the healing amount to the player's health.
        Health += amount;

        // I prevent the health from going above the maximum health.
        Health = Mathf.Clamp(Health, 0, maxHealth);

        // I update the UI after the health has changed.
        UpdateUI();
    }

    // This function removes fuel from the player.
    public void losefuel(float amount)
    {
        // I subtract the amount of fuel used from the current fuel.
        Fuel -= amount;

        // I make sure the fuel does not go below zero or above its maximum value.
        Fuel = Mathf.Clamp(Fuel, 0, maxFuel);

        // I update the UI to show the remaining fuel.
        UpdateUI();
    }

    // This function adds fuel to the player.
    public void addFuel(int amount)
    {
        // I increase the player's fuel by the amount supplied.
        Fuel += amount;

        // I make sure the fuel does not exceed the maximum fuel value.
        Fuel = Mathf.Clamp(Fuel, 0, maxFuel);

        // I update the UI after adding fuel.
        UpdateUI();
    }

    // This function removes money from the player's total.
    public void loseMoney(int amount)
    {
        // I subtract the amount from the player's current money.
        Money -= amount;

        // I make sure the money does not go below zero or above the maximum.
        Money = Mathf.Clamp(Money, 0, maxMoney);

        // I update the UI to show the new money amount.
        UpdateUI();
    }

    // This function adds money to the player's total.
    public void addMoney(int amount)
    {
        // I increase the player's money by the amount supplied.
        Money += amount;

        // I make sure the money stays within the allowed limits.
        Money = Mathf.Clamp(Money, 0, maxMoney);

        // I update the UI after adding money.
        UpdateUI();
    }

    // This function increases the player's score.
    public void addScore(int amount)
    {
        // I add the given amount to the current score.
        Score += amount;

        // I update the UI so the new score can be displayed.
        UpdateUI();
    }

    // This function updates the money text on the screen.
    private void UpdateMoneyDisplay()
    {
        // I check if the money display has been assigned before changing its text.
        if (moneyDisplay != null)
        {
            // I display the Money label followed by the player's current money.
            moneyDisplay.text = "Money: " + Money;
        }
    }

    // This function updates the score text on the screen.
    private void UpdateScoreDisplay()
    {
        // I check if the score display exists before updating it.
        if (scoreDisplay != null)
        {
            // I display the Score label followed by the player's current score.
            scoreDisplay.text = "Score: " + Score;
        }
    }

    // This function refreshes the player's UI when it needs to be updated or reset.
    public void RefreshUI()
    {
        // I check if the health slider exists before setting its values.
        if (HealthSlider != null)
        {
            // I set the minimum, maximum and current health values of the slider.
            HealthSlider.minValue = 0;
            HealthSlider.maxValue = maxHealth;
            HealthSlider.value = Health;
        }

        // I check if the fuel slider exists before setting its values.
        if (FuelSlider != null)
        {
            // I set the minimum, maximum and current fuel values of the slider.
            FuelSlider.minValue = 0;
            FuelSlider.maxValue = maxFuel;
            FuelSlider.value = Fuel;
        }

        // I refresh the money and score displays.
        UpdateMoneyDisplay();
        UpdateScoreDisplay();

        // I check if the GameManager exists before asking it to refresh its UI.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ForceRefresh();
        }
    }
}