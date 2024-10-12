using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    //ViewModels sao modelo baseados em visualizacoes...sao modelos adaptados pra visualizacoes
    //Modelo pra entrada de dados
    //ViewModel tem acoes sobre elas
    //Livra o Model responsabilidade (que eh o sera usado pelo banco),
    //e o ViewModel ficara focado no recebimento via tela/ou Postman por ex.
    public class EditorCategoryViewModel
    {
        [Required] //this field is required
        [StringLength(40, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Slug is required")]
        public string Slug { get; set; }
    }
}
