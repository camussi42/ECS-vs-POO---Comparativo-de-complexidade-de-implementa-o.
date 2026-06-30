using UnityEngine;

/// No OOP, estado (dados) e comportamento (lógica) residem na MESMA classe,
/// acoplados pela herança de MonoBehaviour.
public class BolinhaFisicaOOP : MonoBehaviour
{
    // ── DADOS (estado do objeto) ──────────────────────────────
    // Campo público para permitir inicialização externa pelo SpawnerOOP
    [HideInInspector] public Vector3 velocidade;

    // Constantes de física 
    private const float Gravidade             = -9.8f;
    private const float Elasticidade          =  0.8f;   // coeficiente de restituição
    private const float LimiteChao            =  0f;
    private const float VelocidadeMinima      =  0.1f;   // threshold para parar micro-quiques

    private Transform _transform;

    // ── CICLO DE VIDA

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
            // Projeta a posição de volta ao plano 
            Vector3 posCorrigida = _transform.position;
            posCorrigida.y = LimiteChao;
            _transform.position = posCorrigida;

            // Inverte e atenua a componente vertical 
            velocidade.y = -velocidade.y * Elasticidade;

            // Zera a velocidade se for insignificante 
            if (Mathf.Abs(velocidade.y) < VelocidadeMinima)
            {
                velocidade.y = 0f;
            }
        }
    }
}
