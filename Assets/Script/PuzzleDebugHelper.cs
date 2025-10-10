using UnityEngine;

public class PuzzleDebugHelper : MonoBehaviour
{
    [Header("Ferramentas de Debug do Puzzle")]
    [Tooltip("Arraste o ManagerPecas aqui se não for encontrado automaticamente")]
    public QuadroPieceManager puzzleManager;
    
    [Header("Solução Manual")]
    [Tooltip("Se estiver tendo problemas, atribua manualmente")]
    public bool usarAtribuicaoManual = false;
    
    [Header("Versão Corrigida")]
    [Tooltip("Use esta versão se a original não funcionar")]
    public QuadroPieceManager_Fixed puzzleManagerFixed;

    void Start()
    {
        // Encontra o manager automaticamente se não foi atribuído
        if (puzzleManager == null)
        {
            puzzleManager = FindObjectOfType<QuadroPieceManager>();
            
            if (puzzleManager != null)
            {
                Debug.Log($"PuzzleManager encontrado automaticamente: {puzzleManager.gameObject.name}");
            }
            else
            {
                Debug.LogError("Não foi possível encontrar QuadroPieceManager na cena!");
                
                // Tenta encontrar pelo nome do GameObject
                GameObject managerObj = GameObject.Find("ManagerPecas");
                if (managerObj != null)
                {
                    puzzleManager = managerObj.GetComponent<QuadroPieceManager>();
                    if (puzzleManager != null)
                    {
                        Debug.Log($"PuzzleManager encontrado pelo nome: {managerObj.name}");
                    }
                    else
                    {
                        Debug.LogError("GameObject ManagerPecas existe mas não tem QuadroPieceManager!");
                    }
                }
                else
                {
                    Debug.LogError("GameObject ManagerPecas não encontrado!");
                }
            }
        }
    }

    [ContextMenu("0. Verificar Manager")]
    public void VerificarManager()
    {
        Debug.Log("=== VERIFICANDO PUZZLE MANAGER ===");
        
        // Busca novamente
        QuadroPieceManager[] managers = FindObjectsOfType<QuadroPieceManager>();
        Debug.Log($"Encontrados {managers.Length} QuadroPieceManager(s) na cena");
        
        for (int i = 0; i < managers.Length; i++)
        {
            Debug.Log($"Manager {i}: {managers[i].gameObject.name} - Ativo: {managers[i].gameObject.activeInHierarchy}");
        }
        
        // Busca por nome específico
        GameObject managerObj = GameObject.Find("ManagerPecas");
        if (managerObj != null)
        {
            Debug.Log($"GameObject ManagerPecas encontrado - Ativo: {managerObj.activeInHierarchy}");
            QuadroPieceManager manager = managerObj.GetComponent<QuadroPieceManager>();
            if (manager != null)
            {
                puzzleManager = manager;
                Debug.Log("QuadroPieceManager encontrado e atribuído!");
            }
            else
            {
                Debug.LogError("ManagerPecas não tem o componente QuadroPieceManager!");
            }
        }
        else
        {
            Debug.LogError("GameObject ManagerPecas não encontrado!");
        }
    }

    [ContextMenu("1. Diagnosticar Peças")]
    public void DiagnosticarPeças()
    {
        Debug.Log("=== DIAGNÓSTICO DAS PEÇAS DO PUZZLE ===");
        
        for (int i = 1; i <= 8; i++)
        {
            GameObject peça = GameObject.Find("Peça " + i);
            if (peça != null)
            {
                Debug.Log($"Peça {i}:");
                Debug.Log($"  - Tag: {peça.tag}");
                Debug.Log($"  - Layer: {LayerMask.LayerToName(peça.layer)}");
                Debug.Log($"  - Ativa: {peça.activeInHierarchy}");
                
                // Verifica componentes
                CollectibleItem collectible = peça.GetComponent<CollectibleItem>();
                QuadroPieceCollectable puzzlePiece = peça.GetComponent<QuadroPieceCollectable>();
                
                Debug.Log($"  - Tem CollectibleItem: {collectible != null}");
                Debug.Log($"  - Tem QuadroPieceCollectable: {puzzlePiece != null}");
                
                if (collectible != null)
                {
                    Debug.Log($"  - isPuzzlePiece: {collectible.isPuzzlePiece}");
                }
            }
            else
            {
                Debug.LogWarning($"Peça {i} não encontrada!");
            }
        }
    }

    [ContextMenu("2. Corrigir Todas as Peças")]
    public void CorrigirTodasAsPeças()
    {
        Debug.Log("=== CORRIGINDO PEÇAS DO PUZZLE ===");
        
        for (int i = 1; i <= 8; i++)
        {
            GameObject peça = GameObject.Find("Peça " + i);
            if (peça != null)
            {
                // Corrige tag
                if (!peça.CompareTag("Item"))
                {
                    peça.tag = "Item";
                    Debug.Log($"Tag da {peça.name} corrigida para 'Item'");
                }
                
                // Corrige layer
                int interactionLayer = LayerMask.NameToLayer("Interação");
                if (peça.layer != interactionLayer)
                {
                    peça.layer = interactionLayer;
                    Debug.Log($"Layer da {peça.name} corrigida para 'Interação'");
                }
                
                // Remove CollectibleItem conflitante
                CollectibleItem collectible = peça.GetComponent<CollectibleItem>();
                if (collectible != null)
                {
                    DestroyImmediate(collectible);
                    Debug.Log($"CollectibleItem removido da {peça.name}");
                }
                
                // Garante que tenha collider não-trigger
                Collider col = peça.GetComponent<Collider>();
                if (col != null)
                {
                    col.isTrigger = false;
                }
            }
        }
        
        Debug.Log("Correção concluída! Agora ative o sistema do puzzle.");
    }

    [ContextMenu("3. Ativar Sistema do Puzzle")]
    public void AtivarSistemaPuzzle()
    {
        // Verifica novamente se o manager existe
        if (puzzleManager == null)
        {
            VerificarManager();
        }
        
        if (puzzleManager != null)
        {
            puzzleManager.ForcaAtivacaoAgora();
            Debug.Log("Sistema do puzzle ativado!");
        }
        else
        {
            Debug.LogError("PuzzleManager não encontrado! Execute 'Verificar Manager' primeiro.");
        }
    }

    [ContextMenu("4. Resetar Sistema Completo")]
    public void ResetarSistemaCompleto()
    {
        // Verifica novamente se o manager existe
        if (puzzleManager == null)
        {
            VerificarManager();
        }
        
        if (puzzleManager != null)
        {
            puzzleManager.ResetarSistema();
            Debug.Log("Sistema resetado! Execute 'Corrigir Todas as Peças' e depois 'Ativar Sistema do Puzzle'");
        }
        else
        {
            Debug.LogError("PuzzleManager não encontrado! Execute 'Verificar Manager' primeiro.");
        }
    [ContextMenu("5. Status do Sistema")]
    void MostrarStatusSistema()
    {
        Debug.Log("=== STATUS DO SISTEMA ===");
        
        // Verifica se o manager existe
        if (puzzleManager == null && puzzleManagerFixed == null)
        {
            VerificarManager();
        }
        
        if (puzzleManagerFixed != null)
        {
            // Usa reflection para acessar campos privados para debug
            var tipoManager = puzzleManagerFixed.GetType();
            var sistemaAtivado = tipoManager.GetField("sistemaAtivado", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var pecasColetadas = tipoManager.GetField("pecasColetadas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var puzzleCompleto = tipoManager.GetField("puzzleCompleto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var segundoEvento = tipoManager.GetField("segundoEvento", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Debug.Log($"Sistema Ativado: {sistemaAtivado?.GetValue(puzzleManagerFixed)}");
            Debug.Log($"Peças Coletadas: {pecasColetadas?.GetValue(puzzleManagerFixed)}");
            Debug.Log($"Puzzle Completo: {puzzleCompleto?.GetValue(puzzleManagerFixed)}");
            Debug.Log($"Segundo Evento Configurado: {segundoEvento?.GetValue(puzzleManagerFixed) != null}");
        }
        else if (puzzleManager != null)
        {
            // Usa reflection para acessar campos privados para debug
            var tipoManager = puzzleManager.GetType();
            var sistemaAtivado = tipoManager.GetField("sistemaAtivado", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var pecasColetadas = tipoManager.GetField("pecasColetadas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var puzzleCompleto = tipoManager.GetField("puzzleCompleto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var segundoEvento = tipoManager.GetField("segundoEvento", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Debug.Log($"Sistema Ativado: {sistemaAtivado?.GetValue(puzzleManager)}");
            Debug.Log($"Peças Coletadas: {pecasColetadas?.GetValue(puzzleManager)}");
            Debug.Log($"Puzzle Completo: {puzzleCompleto?.GetValue(puzzleManager)}");
            Debug.Log($"Segundo Evento Configurado: {segundoEvento?.GetValue(puzzleManager) != null}");
        }
        else
        {
            Debug.LogError("Nenhum PuzzleManager encontrado! Execute 'Verificar Manager' primeiro.");
        }
        }
    }
    public void ConfigurarSegundoEvento()
    {
        Debug.Log("=== CONFIGURANDO SEGUNDO EVENTO ===");
        
        // Verifica se o manager existe
        if (puzzleManager == null && puzzleManagerFixed == null)
        {
            VerificarManager();
        }
        
        // Procura pelo evento da sala (normalmente o segundo)
        GameObject eventoSala = GameObject.Find("Evento da Sala");
        if (eventoSala != null)
        {
            EnemyTriggerEvent triggerEvent = eventoSala.GetComponent<EnemyTriggerEvent>();
            if (triggerEvent != null)
            {
                if (puzzleManagerFixed != null)
                {
                    // Usa reflection para configurar o segundoEvento
                    var tipoManager = puzzleManagerFixed.GetType();
                    var campoSegundoEvento = tipoManager.GetField("segundoEvento", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (campoSegundoEvento != null)
                    {
                        campoSegundoEvento.SetValue(puzzleManagerFixed, triggerEvent);
                        Debug.Log("Segundo evento configurado com sucesso: " + eventoSala.name);
                    }
                }
                else if (puzzleManager != null)
                {
                    // Usa reflection para configurar o segundoEvento
                    var tipoManager = puzzleManager.GetType();
                    var campoSegundoEvento = tipoManager.GetField("segundoEvento", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (campoSegundoEvento != null)
                    {
                        campoSegundoEvento.SetValue(puzzleManager, triggerEvent);
                        Debug.Log("Segundo evento configurado com sucesso: " + eventoSala.name);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Evento da Sala não encontrado!");
        }
    }

    [ContextMenu("7. Ativar Sistema Manualmente (Ignorar Evento)")]
    public void AtivarSistemaManualmente()
    {
        Debug.Log("=== ATIVANDO SISTEMA MANUALMENTE ===");
        
        // Verifica se o manager existe
        if (puzzleManager == null && puzzleManagerFixed == null)
        {
            VerificarManager();
        }
        
        if (puzzleManagerFixed != null)
        {
            puzzleManagerFixed.AtivarSistemaPecas();
            Debug.Log("Sistema ativado manualmente com QuadroPieceManager_Fixed!");
        }
        else if (puzzleManager != null)
        {
            puzzleManager.AtivarSistemaPecas();
            Debug.Log("Sistema ativado manualmente com QuadroPieceManager!");
        }
        else
        {
            Debug.LogError("Nenhum manager encontrado!");
        }
    }
}