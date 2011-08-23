﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Apiki_Buscape_API
{
    public class FiltrosFindOfferList
    {
        private int categoryId;
        private int productId;
        private int results;
        private int page;
        private float priceMin;
        private float priceMax;
        private bool isLomadee;
        private string keyword;
        private string barcode;
        private string callback;
        private string sort;
        private string medal;
        
        /// <summary>
        /// Filtra ofertas e produtos a partir de uma determinada medalha do eBit:
        ///    * all
        ///    * diamond
        ///    * gold
        ///    * silver
        ///    * bronze
        /// </summary>
        public string Medal
        {
            get { return medal; }
            set { medal = value; }
        }

        /// <summary>
        /// Ordenação dos resultados:
        ///    * price/dprice: Ordenação por preço.
        ///    * rate/drate: Ordenação por avaliação de usuários.
        ///    * seller/dseller: Ordenação por loja.
        ///    * installment/dinstallment: Ordenação por preço da parcela.
        ///    * numberofinstallments/dnumberofinstallments: Ordenação por número de parcelas.
        ///    * trustedStore: Ordenação por selo de empresa reconhecida no BuscaPé.
        /// </summary>
        public string Sort
        {
            get { return sort; }
            set { sort = value; }
        }

        /// <summary>
        /// Preço Máximo de pesquisa.
        /// </summary>
        public float PriceMax
        {
            get { return priceMax; }
            set { priceMax = value; }
        }

        /// <summary>
        /// Preço Mínimo de pesquisa.
        /// </summary>
        public float PriceMin
        {
            get { return priceMin; }
            set { priceMin = value; }
        }

        /// <summary>
        /// Número da página.
        /// </summary>
        public int Page
        {
            get { return page; }
            set { page = value; }
        }

        /// <summary>
        /// Número de resultados retornado em cada requisição.
        /// </summary>
        public int Results
        {
            get { return results; }
            set { results = value; }
        }

        /// <summary>
        /// Indica se será uma requisição ao serviço da lomadee.
        /// </summary>
        public bool IsLomadee
        {
            get { return isLomadee; }
            set { isLomadee = value; }
        }

        /// <summary>
        /// Função de retorno a ser executada caso esteja usando json.
        /// </summary>
        public string Callback
        {
            get { return callback; }
            set { callback = value; }
        }

        /// <summary>
        /// Código de barras do produto.
        /// </summary>
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }

        /// <summary>
        /// Código do produto.
        /// </summary>
        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }

        /// <summary>
        /// Palavra chave para busca entre as categorias.
        /// </summary>
        public string Keyword
        {
            get { return keyword; }
            set { keyword = value; }
        }

        /// <summary>
        /// Código da categoria de produtos para se pesquisar as ofertas.
        /// </summary>
        public int CategoryId
        {
            get { return categoryId; }
            set { categoryId = value; }
        }

        /// <summary>
        /// Usa as propriedades do objeto que foram setadas com algum valor para montar os parâmetros
        /// da URL de requisição à API do BuscaPé.
        /// </summary>
        /// <returns>Uma string com os parâmetros já montados para a URL.</returns>
        public string MakeUrlParameters()
        {
            /*
            string[] parametros = new string[12];
            parametros[0] = this.categoryId.ToString();
            parametros[1] = this.keyword;

            string s = string.Join("&", parametros);
            */

            string param = string.Empty;

            /* Primeiro checamos os parâmetros obrigatórios */
            if (this.categoryId != 0)
                param = "?categoryId=" + this.categoryId.ToString();
            if (!string.IsNullOrEmpty(this.keyword))
            {
                if (!string.IsNullOrEmpty(param))
                    param += "&keyword=" + this.keyword;
                else
                    param = "?keyword=" + this.keyword;
            }
            if (this.productId != 0)
                param = "?productId=" + this.productId.ToString();
            if (!string.IsNullOrEmpty(this.barcode))
                param = "?barcode=" + this.barcode;

            if (string.IsNullOrEmpty(param))
                return string.Empty;

            /* Agora efetuamos os filtros adicionais informados */
            if (this.results != 0)
                param += "&results=" + this.results;

            if (this.page != 0)
                param += "&page=" + this.page;

            if (this.priceMin != 0.0)
                param += "&priceMin=" + this.priceMin;

            if (this.priceMax != 0.0)
                param += "&priceMax=" + this.priceMax;

            string[] validSort = new string[] { "price", "dprice", "rate", "drate", "seller", "dseller",
                                                "installment", "dinstallment", "numberofinstallments",
                                                "dnumberofinstallments", "trustedStore" };
            if (validSort.Contains(this.sort))
                param += "&sort=" + this.sort;

            string[] validMedal = new string[] { "all", "diamond", "gold", "silver", "bronze" };
            if (validMedal.Contains(this.medal))
                param += "&medal=" + this.medal;
            
            return param;

        }
    }
}