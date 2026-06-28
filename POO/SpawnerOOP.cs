// =============================================================
// SpawnerOOP.cs  —  Paradigma: Programação Orientada a Objetos
// Gerenciador que instancia N instâncias de BolinhaFisicaOOP
// =============================================================
// ACOPLAMENTO (CBO): esta classe DEPENDE diretamente de:
//   - BolinhaFisicaOOP  (GetComponent + atribuição de campo)
//   - UnityEngine.GameObject
//   - UnityEngine.MonoBehaviour (herança)
// Esse acoplamento estrutural será capturado pela métrica CBO no SonarQube.
// =============================================================

using UnityEngine;

/// <summary>
/// Responsável por instanciar e inicializar N bolinhas no início da cena.
/// Demonstra o acoplamento característico do OOP: o Spawner conhece
/// e manipula diretamente o estado interno de BolinhaFisicaOOP.
/// </summary>
public class SpawnerOOP : MonoBehaviour
{
    // ── CONFIGURAÇÃO (via Inspector) ──────────────────────────
    [SerializeField] private GameObject prefabBolinha;
    [SerializeField] private int        quantidade  = 1000;
    [SerializeField] private float      areaSpawn   = 10f;
    [SerializeField] private float      alturaMin   = 5f;
    [SerializeField] private float      alturaMax   = 15f;
    [SerializeField] private float      velHorizMax = 3f;

    // ── CICLO DE VIDA ─────────────────────────────────────────

    private void Start()
    {
        // Pré-aloca uma lista de GameObjects para reduzir realocações de heap
        // (sem impacto em runtime após Start, mas demonstra intenção de otimização)
        for (int i = 0; i < quantidade; i++)
        {
            InstanciarBolinha();
        }
    }

    /// <summary>
    /// Instancia uma bolinha em posição e velocidade aleatórias.
    /// PONTO DE ACOPLAMENTO: Spawner acessa diretamente o componente
    /// BolinhaFisicaOOP e modifica seu campo `velocidade` — característica
    /// de acoplamento forte (CBO elevado) típica do OOP.
    /// </summary>
    private void InstanciarBolinha()
    {
        Vector3 posicaoInicial = new Vector3(
            Random.Range(-areaSpawn, areaSpawn),
            Random.Range(alturaMin,  alturaMax),
            Random.Range(-areaSpawn, areaSpawn)
        );

        // Instanciação padrão do Unity: cria GameObject + todos os seus Components
        GameObject go = Instantiate(prefabBolinha, posicaoInicial, Quaternion.identity);

        // Acoplamento direto: Spawner conhece e manipula BolinhaFisicaOOP
        BolinhaFisicaOOP bolinha = go.GetComponent<BolinhaFisicaOOP>();
        if (bolinha != null)
        {
            bolinha.velocidade = new Vector3(
                Random.Range(-velHorizMax, velHorizMax),
                0f,
                Random.Range(-velHorizMax, velHorizMax)
            );
        }
    }
}
