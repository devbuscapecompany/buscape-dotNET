using System.IO;
using System.Linq;
using System.Net;

namespace Apiki_Buscape_API
{
    /// <summary>
    /// A classe Apiki_Buscape_API foi criada para ajudar no desenvolvimento de
    /// aplicações usando os webservices disponibilizados pela API do BuscaPé©.
    /// 
    /// Os métodos desta classe tem os mesmos nomes dos serviços disponibilizados pelo
    /// BuscaPé©.
    ///         
    /// </summary>
    /// <author>Apiki</author>
    /// <version>1.0</version>
    /// <license>Creative Commons Atribuição 3.0 Brasil. http://creativecommons.org/licenses/by/3.0/br/</license>
    public class Apiki_Buscape_API
    {
        #region Atributos/Propriedades

        /// <summary>
        /// ID da aplicação
        /// </summary>
        private string applicationId;

        /// <summary>
        /// Código do país
        /// </summary>
        private string countryCode;

        /// <summary>
        /// Formato de retorno
        /// </summary>
        private string format;

        /// <summary>
        /// Usando JSON
        /// </summary>
        private string isJson;

        /// <summary>
        /// Ambiente servidor ( sandbox | bws )
        /// </summary>
        private string server = "sandbox";

        /// <summary>
        /// Source ID do lomadee, com o qual a API do BuscaPé irá vincular com o perfil Lomadee.
        /// </summary>
        private string sourceId;

        /// <summary>
        /// Identificação dos serviços executados pela classe
        /// </summary>
        private enum Services
        {
            findAdvertiserList,
            findCategoryList,
            findProductList,
            findOfferList,
            topProducts,
            viewUserRatings,
            viewProductDetails,
            viewSellerDetails,
            createSource,
            saveCode,
            getCode
        }

        #endregion

        #region Construtor

        public Apiki_Buscape_API(string applicationId) : this(applicationId, string.Empty) { }

        public Apiki_Buscape_API(string applicationId, string sourceId) : this(applicationId, sourceId, "BR") { }

        public Apiki_Buscape_API(string applicationId, string sourceId, string countryCode) : this(applicationId, sourceId, countryCode, "xml") { }

        public Apiki_Buscape_API(string applicationId, string sourceId, string countryCode, string format) : this(applicationId, sourceId, countryCode, format, true) { }

        public Apiki_Buscape_API(string applicationId, string sourceId, string countryCode, string format, bool sandbox)
        {
            this.applicationId = applicationId;
            this.countryCode = countryCode;
            this.format = format;
            this.isJson = (this.format.Equals("json")) ? "&format=json" : string.Empty;
            this.sourceId = sourceId;

            if (!sandbox)
                this.server = "bws";

            if (string.IsNullOrEmpty(this.applicationId))
                this.ShowErrors("ID da aplicação não pode ser vazio.");

            if (!(new string[] { "AR", "BR", "CL", "CO", "MX", "PE", "VE" }.Contains(countryCode)))
                this.ShowErrors(string.Format("O código do país <b>{0}</b> não existe.", this.countryCode));

            if (!(new string[] { "xml", "json" }.Contains(this.format)))
                this.ShowErrors(string.Format("O formato de retorno <b>{0}</b> não existe.", this.format));
        }

        #endregion

        #region FindAdvertiserList

        /// <summary>
        /// Recupera dados dos anunciantes.
        /// </summary>
        /// <param name="siteId">Código do site na Lomadee.</param>
        /// <param name="publisherId">Código do publisher na Lomadee.</param>
        /// <param name="token">Token para validação do publisher.</param>
        /// <param name="callback">Função de retorno para o json.</param>
        /// <returns>Retorna uma string com os dados dos anunciantes.</returns>
        public string FindAdvertiserList(string siteId, string publisherId, string token, string callback)
        {
            string param = string.Empty;

            if (siteId == string.Empty)
                this.ShowErrors(string.Format("O id do site é requerido na função <b>{0}</b>.", Services.findAdvertiserList));
            if (publisherId == string.Empty)
                this.ShowErrors(string.Format("O id do publisher é requerido na função <b>{0}</b>.", Services.findAdvertiserList));
            if (token == string.Empty)
                this.ShowErrors(string.Format("O token é requerido na função <b>{0}</b>.", Services.findAdvertiserList));

            param = "?siteId=" + siteId + "&publisherId=" + publisherId + "&token=" + token;
            if (callback != string.Empty && this.format == "json")
                param += "&callback=" + callback;

            string url = string.Format("http://{0}.buscape.com/service/{1}/lomadee/{2}/{3}/{4}", this.server, Services.findAdvertiserList, this.applicationId, this.countryCode, param);

            return this.GetContent(url);
        }

        #endregion

        #region FindCategoryList

        /// <summary>
        /// Recupera dados das categorias. Para o parâmetro categoryID pode ser informado
        /// o valor 0(zero) retornando assim uma lista com as categorias raiz.
        /// Caso o código da categoria seja passado null e nenhuma palavra-chave tenha sido 
        /// informada, será retornado uma lista com as categorias raiz.
        /// </summary>
        /// <param name="categoryId">ID da categoria</param>
        /// <param name="keyword">Palavra chave para busca entre as categorias</param>
        /// <param name="callback">Função de retorno a ser executada caso esteja usando json</param>
        /// <returns>Retorna uma string com os dados das categorias.</returns>
        public string FindCategoryList(int? categoryId, string keyword, string callback)
        {
            string param = string.Empty;

            if (categoryId == null && keyword == string.Empty)
                categoryId = 0;

            if (categoryId != null)
                param = "?categoryId=" + categoryId.ToString();

            if (!string.IsNullOrEmpty(keyword))
            {
                if (param != string.Empty)
                    param += "&keyword=" + keyword;
                else
                    param = "?keyword=" + keyword;
            }

            param += (!string.IsNullOrEmpty(callback)) ? "&callback=" + callback : string.Empty;
            param += (!string.IsNullOrEmpty(this.sourceId)) ? "&sourceId=" + this.sourceId : string.Empty;
            param += this.isJson;

            string url = string.Format("http://{0}.buscape.com/service/{1}/{2}/{3}/{4}", this.server, Services.findCategoryList, this.applicationId, this.countryCode, param);

            return this.GetContent(url);
        }

        #endregion

        #region FindOfferList

        /// <summary>
        /// Recupera uma lista de ofertas.
        /// </summary>
        /// <remarks>
        /// Pelo menos um dos parametros de pesquisa devem ser informados para o 
        /// retorno da função. Os parâmetros categoryId e keyword podem ser usados em conjunto.
        /// Quando um parâmetro for usado, os outros devem ser setados como string.empty.
        /// </remarks>
        /// <param name="categoryId">Código da categoria</param>
        /// <param name="keyword">Palavra chave para busca entre as categorias</param>
        /// <param name="productId">Código do produto</param>
        /// <param name="barcode">Código de barras do produto</param>
        /// <param name="callback">Função de retorno a ser executada caso esteja usando json</param>
        /// <returns>Retorna uma string com os dados das ofertas</returns>
        public string FindOfferList(int categoryId, string keyword, int productId, string barcode, string callback)
        {
            return this.FindOfferList(categoryId, keyword, productId, barcode, callback, false);
        }

        /// <summary>
        /// Recupera uma lista de ofertas.
        /// </summary>
        /// <remarks>
        /// Pelo menos um dos parametros de pesquisa devem ser informados para o 
        /// retorno da função. Os parâmetros categoryId e keyword podem ser usados em conjunto.
        /// Quando um parâmetro for usado, os outros devem ser setados como string.empty.
        /// </remarks>
        /// <param name="categoryId">Código da categoria</param>
        /// <param name="keyword">Palavra chave para busca entre as categorias</param>
        /// <param name="productId">Código do produto</param>
        /// <param name="barcode">Código de barras do produto</param>
        /// <param name="callback">Função de retorno a ser executada caso esteja usando json</param>
        /// <param name="isLomadee">Indica se será uma requisição ao serviço da lomadee</param>
        /// <returns>Retorna uma string com os dados das ofertas</returns>
        public string FindOfferList(int categoryId, string keyword, int productId, string barcode, string callback, bool isLomadee)
        {
            string param = string.Empty;
            string paramLomadee = string.Empty;

            if (categoryId != 0)
                param = "?categoryId=" + categoryId.ToString();
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!string.IsNullOrEmpty(param))
                    param += "&keyword=" + keyword;
                else
                    param = "?keyword=" + keyword;
            }
            if (productId != 0)
                param = "?productId=" + productId.ToString();
            if (!string.IsNullOrEmpty(barcode))
                param = "?barcode=" + barcode;

            if (string.IsNullOrEmpty(param))
                this.ShowErrors("Pelo menos um parâmetro de pesquisa é requerido na função " + Services.findOfferList);

            param += (!string.IsNullOrEmpty(this.sourceId)) ? "&sourceId=" + this.sourceId : string.Empty;
            paramLomadee = (isLomadee) ? "/lomadee" : string.Empty;

            string url = string.Format("http://{0}.buscape.com/service/{1}{5}/{2}/{3}/{4}", this.server, Services.findOfferList, this.applicationId, this.countryCode, param, paramLomadee);

            return GetContent(url);
        }

        /// <summary>
        /// Recupera uma lista de ofertas.
        /// </summary>
        /// <param name="filtros">Objeto do tipo FiltrosFindOfferList que contém todos as opções de filtragem desejadas.</param>
        /// <returns>Retorna uma string com os dados das ofertas</returns>
        public string FindOfferList(FiltrosFindOfferList filtros)
        {
            string param = filtros.MakeUrlParameters();
            string paramLomadee = string.Empty;

            if (string.IsNullOrEmpty(param))
                this.ShowErrors("Pelo menos um parâmetro de pesquisa é requerido na função " + Services.findOfferList);

            param += (!string.IsNullOrEmpty(this.sourceId)) ? "&sourceId=" + this.sourceId : string.Empty;
            paramLomadee = (filtros.IsLomadee) ? "/lomadee" : string.Empty;

            string url = string.Format("http://{0}.buscape.com/service/{1}{5}/{2}/{3}/{4}", this.server, Services.findOfferList, this.applicationId, this.countryCode, param, paramLomadee);

            return GetContent(url);
        }

        #endregion

        #region FindProductList

        /// <summary>
        /// Recupera uma lista de produtos únicos
        /// </summary>
        /// <param name="categoryId">ID da Categoria</param>
        /// <param name="keyword">Palavra chave para busca entre as categorias</param>
        /// <param name="callback">Função de retorno a ser executada caso esteja usando json</param>
        /// <returns>Retorna uma string com os dados dos produtos</returns>
        public string FindProductList(int categoryId, string keyword, string callback)
        {
            return this.FindProductList(categoryId, keyword, callback, false);
        }

        /// <summary>
        /// Recupera uma lista de produtos únicos
        /// </summary>
        /// <param name="categoryId">ID da Categoria</param>
        /// <param name="keyword">Palavra chave para busca entre as categorias</param>
        /// <param name="callback">Função de retorno a ser executada caso esteja usando json</param>
        /// <param name="isLomadee">Indica se será uma requisição ao serviço da lomadee</param>
        /// <returns>Retorna uma string com os dados dos produtos</returns>
        public string FindProductList(int categoryId, string keyword, string callback, bool isLomadee)
        {
            string param = string.Empty;
            string paramLomadee = string.Empty;

            if (categoryId != 0)
                param = "?categoryId=" + categoryId.ToString();
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!string.IsNullOrEmpty(param))
                    param += "&keyword=" + keyword;
                else
                    param = "?keyword=" + keyword;
            }
            if (string.IsNullOrEmpty(param))
                this.ShowErrors("Pelo menos um parâmetro de pesquisa é requerido na função " + Services.findProductList);

            param += (!string.IsNullOrEmpty(callback)) ? "&callback=" + callback : string.Empty;
            param += (!string.IsNullOrEmpty(this.sourceId)) ? "&sourceId=" + this.sourceId : string.Empty;
            param += this.isJson;
            paramLomadee = (isLomadee) ? "/lomadee" : string.Empty;

            string url = string.Format("http://{0}.buscape.com/service/{1}{5}/{2}/{3}/{4}", this.server, Services.findProductList, this.applicationId, this.countryCode, param, paramLomadee);

            return GetContent(url);
        }

        #endregion

        #region TopProducts

        /// <summary>
        /// Recupera os produtos mais populares no BúscaPé
        /// </summary>
        /// <param name="callback">Função de retorno a ser executada caso esteja usando json</param>
        /// <returns>Retorna uma string com os dados dos produtos</returns>
        public string TopProducts(string callback)
        {
            string param = string.Empty;
            param = (this.format.Equals("json")) ? "?format=json" : string.Empty;
            param += (!string.IsNullOrEmpty(param) && !string.IsNullOrEmpty(callback)) ? "&callback=" + callback : string.Empty;
            if (!string.IsNullOrEmpty(param))
                param += (!string.IsNullOrEmpty(this.sourceId)) ? "&sourceId=" + this.sourceId : string.Empty;
            else
                param += (!string.IsNullOrEmpty(this.sourceId)) ? "?sourceId=" + this.sourceId : string.Empty;

            string url = string.Format("http://{0}.buscape.com/service/{1}/{2}/{3}/{4}", this.server, Services.topProducts, this.applicationId, this.countryCode, param);

            return GetContent(url);
        }

        #endregion

        #region ViewProductDetails

        /// <summary>
        /// Recupera detalhes técnicos de um determinado produto
        /// </summary>
        /// <param name="productId">Id do produto</param>
        /// <param name="callback">Função de retorno a ser executada caso esteja usando json</param>
        /// <returns>Retorna uma string contendo os dados do produt</returns>
        public string ViewProductDetails(int productId, string callback)
        {
            string param = string.Empty;

            if (productId != 0)
                param = "?productId=" + productId.ToString();
            else
                this.ShowErrors(string.Format("ID do produto requerido na função <b>{0}</b>.", Services.viewProductDetails));

            param += (!string.IsNullOrEmpty(callback)) ? "&callback=" + callback : string.Empty;
            param += (!string.IsNullOrEmpty(this.sourceId)) ? "&sourceId=" + this.sourceId : string.Empty;
            param += this.isJson;

            string url = string.Format("http://{0}.buscape.com/service/{1}/{2}/{3}/{4}", this.server, Services.viewProductDetails, this.applicationId, this.countryCode, param);

            return GetContent(url);
        }

        #endregion

        #region ViewSellerDetails

        /// <summary>
        /// Recupera detalhes da loja/empresa
        /// </summary>
        /// <param name="sellerId">Código da loja/empresa</param>
        /// <param name="callback">Função de retorno a ser executada caso esteja usando json</param>
        /// <returns>Retorna uma string contendo os dados da loja/empresa no formato especificado(xml/json)</returns>
        public string ViewSellerDetails(int sellerId, string callback)
        {
            string param = string.Empty;

            if (sellerId != 0)
                param = "?sellerId=" + sellerId.ToString();
            else
                this.ShowErrors(string.Format("ID da loja/empresa requerido na função <b>{0}</b>.", Services.viewSellerDetails));

            param += (!string.IsNullOrEmpty(callback)) ? "&callback=" + callback : string.Empty;
            param += (!string.IsNullOrEmpty(this.sourceId)) ? "&sourceId=" + this.sourceId : string.Empty;
            param += this.isJson;

            string url = string.Format("http://{0}.buscape.com/service/{1}/{2}/{3}/{4}", this.server, Services.viewSellerDetails, this.applicationId, this.countryCode, param);

            return GetContent(url);
        }

        #endregion

        #region ViewUserRatings

        /// <summary>
        /// Recupera as avaliações dos usuários sobre um determinado produto
        /// </summary>
        /// <param name="productId">Código do produto</param>
        /// <param name="callback">Função de retorno a ser executada caso esteja usando json</param>
        /// <returns>Retorna uma string com os dados das avaliações</returns>
        public string ViewUserRatings(int productId, string callback)
        {
            string param = string.Empty;

            if (productId != 0)
                param = "?productId=" + productId.ToString();
            else
                this.ShowErrors(string.Format("ID do produto requerido na função <b>{0}</b>.", Services.viewUserRatings));

            param += (!string.IsNullOrEmpty(callback)) ? "&callback=" + callback : string.Empty;
            param += (!string.IsNullOrEmpty(this.sourceId)) ? "&sourceId=" + this.sourceId : string.Empty;
            param += this.isJson;

            string url = string.Format("http://{0}.buscape.com/service/{1}/{2}/{3}/{4}", this.server, Services.viewUserRatings, this.applicationId, this.countryCode, param);

            return GetContent(url);
        }

        #endregion

        #region CreateSource

        /// <summary>
        /// Função utilizada para criar um Source ID para o publisher que desejar integrar o seu aplicativo.
        /// </summary>
        /// <param name="siteId">o id do site do publisher</param>
        /// <param name="publisherId">o id do publisher</param>
        /// <param name="token">token de segurança</param>
        /// <param name="sourceName">o nome do source escolhido pelo publisher</param>
        /// <returns>Retorna uma string com o XML que contém o Source ID.</returns>
        public string CreateSource(string siteId, string publisherId, string token, string sourceName)
        {
            string param = string.Empty;

            if (siteId.Equals(string.Empty) || publisherId.Equals(string.Empty) || token.Equals(string.Empty) || sourceName.Equals(string.Empty))
                this.ShowErrors(string.Format("Todos os parâmetros são requeridos na função <b>{0}</b>.", Services.createSource));

            string[] arr_param = new string[] { "siteId=" + siteId, "publisherId=" + publisherId, "token=" + token, "sourceName=" + sourceName };
            param = "?" + string.Join("&", arr_param);

            string url = string.Format("http://{0}.buscape.com/service/{1}/lomadee/{2}/{3}/{4}", this.server, Services.createSource, this.applicationId, this.countryCode, param);

            return GetContent(url);
        }

        #endregion

        #region GetCode

        /// <summary>
        /// Recupera o código que deverá ser utilizado pelo publisher em seu site.
        /// </summary>
        /// <param name="sourceId">O Source Id gerado com a função CreateSource.</param>
        /// <returns>Retorna uma string com o código a ser implementado no site.</returns>
        public string GetCode(string sourceId)
        {
            string param = string.Empty;

            if (sourceId.Equals(string.Empty))
                this.ShowErrors(string.Format("O parâmetro sourceId é requerido na função <b>{0}</b>.", Services.getCode));

            param = "?sourceId=" + sourceId;

            string url = string.Format("http://{0}.buscape.com/service/{1}/lomadee/{2}/{3}/{4}", this.server, Services.getCode, this.applicationId, this.countryCode, param);

            return GetContent(url);
        }

        #endregion

        #region SaveCode

        /// <summary>
        /// Salva na base da Lomadee o código JS/HTML que deverá ser utilizado pelo publisher.
        /// </summary>
        /// <param name="siteId">o id do site do publisher</param>
        /// <param name="publisherId">o id do publisher</param>
        /// <param name="token">token de segurança</param>
        /// <param name="sourceId">o SourceId gerado para o publisher</param>
        /// <param name="code">O código que será salvo para o publisher</param>
        /// <returns></returns>
        public void SaveCode(string siteId, string publisherId, string token, string sourceId, string code)
        {
            string param = string.Empty;

            if (siteId.Equals(string.Empty) || publisherId.Equals(string.Empty) || token.Equals(string.Empty) || sourceId.Equals(string.Empty) || code.Equals(string.Empty))
                this.ShowErrors(string.Format("Todos os parâmetros são requeridos na função <b>{0}</b>.", Services.saveCode));

            string[] arr_param = new string[] { "siteId=" + siteId, "publisherId=" + publisherId, "token=" + token, "sourceId=" + sourceId, "code=" + code };
            param = "?" + string.Join("&", arr_param);

            string url = string.Format("http://{0}.buscape.com/service/{1}/lomadee/{2}/{3}/{4}", this.server, Services.saveCode, this.applicationId, this.countryCode, param);

            this.PostContent(url);
        }

        #endregion

        #region ShowErrors
        /// <summary>
        /// Dispara uma WebException.
        /// </summary>
        /// <remarks>
        /// O método deve ser invocado sempre que algum erro que impossibilite continuar
        /// seja encontrado.
        /// </remarks>
        /// <param name="message">Mensagem que será exibida junto com a Exception</param>
        private void ShowErrors(string message)
        {
            throw new WebException(message, WebExceptionStatus.RequestCanceled);
        }
        #endregion

        #region GetContent
        /// <summary>
        /// Recupera os dados do serviço BuscaPé.
        /// </summary>
        /// <param name="url">URL para acesso ao serviço</param>
        /// <returns>Uma string com os dados de retorno da URL requisitada</returns>
        private string GetContent(string url)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    using (StreamReader reader = new StreamReader(client.OpenRead(url)))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region PostContent

        /// <summary>
        /// Realiza um HTTP POST nos serviços da Lomadee.
        /// </summary>
        /// <param name="url">URL para acesso ao serviço.</param>
        /// <returns>Uma string com os dados de retorno da URL requisitada.</returns>
        private string PostContent(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post;
                WebResponse response = request.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
