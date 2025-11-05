using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class DiceManager : MonoBehaviour
{
    public List<DiceRoller> diceList;
    public TextMeshProUGUI resultadoText;
    private bool isRolling = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
            RollAllDice();
    }

    void RollAllDice()
    {
        isRolling = true;
        resultadoText.text = "Rodando...";
        foreach (var dice in diceList)
            dice.RollDice();

        StartCoroutine(WaitForAllDice());
    }

    IEnumerator WaitForAllDice()
    {
        yield return new WaitForSeconds(2f);
        while (diceList.Any(d => d.IsRolling()))
            yield return null;

        List<int> results = diceList.Select(d => d.GetResult()).ToList();
        EvaluateRoll(results);
        isRolling = false;
    }

    void EvaluateRoll(List<int> results)
    {
        var counts = results.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        int sum = results.Sum();
        int multiplier = 10;
        string jugada = "Nada útil";

        if (counts.ContainsValue(5)) { jugada = "Generala"; multiplier = 5000; }
        else if (counts.ContainsValue(4)) { jugada = "Poker"; multiplier = 50; }
        else if (counts.ContainsValue(3) && counts.ContainsValue(2)) { jugada = "Full House"; multiplier = 40; }
        else if (counts.ContainsValue(3)) { jugada = "Trío"; multiplier = 30; }
        else if (counts.Count(x => x.Value == 2) == 2) { jugada = "Dos Pares"; multiplier = 30; }
        else if (counts.ContainsValue(2)) { jugada = "Par"; multiplier = 20; }
        else if (results.OrderBy(x => x).SequenceEqual(new List<int> { 1, 2, 3, 4, 5 }) ||
            results.OrderBy(x => x).SequenceEqual(new List<int> { 2, 3, 4, 5, 6 }))
        { jugada = "Escalera"; multiplier = 40; }

        int score = sum * multiplier;
        resultadoText.text = $"Resultados: {string.Join(", ", results)}\n{jugada} - Puntuación: {score}";
    }
}
