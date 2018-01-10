﻿using OpenMined.Network.Controllers;
using OpenMined.Syft.Tensor;
using UnityEngine;
using System;
using OpenMined.Network.Servers;
using Newtonsoft.Json;

namespace OpenMined.Syft.Layer
{
    [Serializable]
    public class Linear: Layer, LayerDefinition
	{
		private int _input;
		private int _output;

        [SerializeField] string name = "linear";
        [SerializeField] readonly FloatTensor _weights;
        [SerializeField] FloatTensor _bias;
		
		public Linear (SyftController _controller, int input, int output, string initializer="Xavier")
		{
            init(this.name);

			this.controller = _controller;
			
			_input = input;
			_output = output;
			
			int[] weightShape = { input, output };
            var weights = initializer == "Xavier" ? controller.RandomWeights(input * output, input) : controller.RandomWeights(input * output);
			_weights = controller.floatTensorFactory.Create(_shape: weightShape, _data: weights, _autograd: true, _keepgrads: true);

			int[] biasShape = {1,output};
			_bias = controller.floatTensorFactory.Create(_shape:biasShape, _autograd: true);

			parameters.Add(_weights.Id);
			parameters.Add(_bias.Id);
			
			#pragma warning disable 420
			id = System.Threading.Interlocked.Increment(ref nCreated);
			controller.addModel(this);

		}

        public override FloatTensor Forward(FloatTensor input)
		{
			
			FloatTensor unbiased_output = input.MM(_weights);
			FloatTensor output = unbiased_output.Add(_bias.Expand(unbiased_output.Shape).Contiguous());
			
			activation = output.Id;
		
			return output;
		}

        public string GetLayerDefinition()
        {
            return JsonUtility.ToJson(this);
        }

        public override int getParameterCount(){return _weights.Size + _bias.Size;}
    }
}

