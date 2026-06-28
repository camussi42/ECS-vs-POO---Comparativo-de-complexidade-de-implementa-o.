// =============================================================
// BolinhaOOP.cs  —  Paradigma: Programação Orientada a Objetos
// Simulação de física com gravidade + quique no plano y = 0
// Padrão de referência: benchmarks de MonoBehaviour vs DOTS
// Compatível com: Unity 6 / 2022 LTS+
// =============================================================

using UnityEngine;

/// <summary>
/// Define dados e comportamento de uma bolinha física no paradigma OOP.
/// No OOP, estado (dados) e comportamento (lógica) residem na MESMA classe,
/// acoplados pela herança de MonoBehaviour.
/// </summary>
public class BolinhaFisicaOOP : MonoBehaviour
{
    // ── DADOS (estado do objeto) ──────────────────────────────
    // Campo público para permitir inicialização externa pelo SpawnerOOP
    [HideInInspector] public Vector3 velocidade;

    // Constantes de física — imutáveis em tempo de execução
    private const float Gravidade             = -9.8f;
    private const float Elasticidade          =  0.8f;   // coeficiente de restituição
    private const float LimiteChao            =  0f;
    private const float VelocidadeMinima      =  0.1f;   // threshold para parar micro-quiques

    // Cache do Transform: evita chamadas P/Invoke (C# → C++) redundantes no hot-path.
    // OTIMIZAÇÃO CRÍTICA: sem cache, cada acesso a `transform` atravessa a interop managed/native.
    private Transform _transform;

    // ── CICLO DE VIDA ─────────────────────────────────────────

    private void Awake()
    {
        // Resolve uma única vez; custo de Awake() é pago apenas na criação do objeto.
        _transform = transform;
    }

    /// <summary>
    /// Executado TODA frame pelo engine, para CADA instância de BolinhaFisicaOOP.
    /// Custo: O(n) chamadas individuais, onde n = número de bolinhas ativas.
    /// Cada chamada é gerenciada e não pode ser paralelizada pelo Unity automaticamente.
    /// </summary>
    private void Update()
    {
        float dt = Time.deltaTime;

        // 1. Integração de Euler semi-implícita: atualiza velocidade com a aceleração
        velocidade.y += Gravidade * dt;

        // 2. Atualiza posição com a velocidade resultante
        _transform.position += velocidade * dt;

        // 3. Detecção e resposta de colisão com o plano y = 0
        if (_transform.position.y < LimiteChao)
        {
            // Projeta a posição de volta ao plano (resolução de penetração)
            Vector3 posCorrigida = _transform.position;
            posCorrigida.y = LimiteChao;
            _transform.position = posCorrigida;

            // Inverte e atenua a componente vertical (lei de restituição de Newton)
            velocidade.y = -velocidade.y * Elasticidade;

            // Zera a velocidade se for insignificante (evita quiques infinitos de amplitude zero)
            if (Mathf.Abs(velocidade.y) < VelocidadeMinima)
            {
                velocidade.y = 0f;
            }
        }
    }
}
