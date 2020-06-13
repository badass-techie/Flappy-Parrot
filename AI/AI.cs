using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NumSharp;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class AI : MonoBehaviour{
    static AI instance = null;
    [HideInInspector] public int speciesAlive;
    int[] sizes = {4, 12, 1 };
    int noOfLayers;
    double randomRange = 1.0;
    public int noOfSpecies;
    public double fitnessThreshold;
    [HideInInspector] public int generation = 1, highestScore = 0;
    [HideInInspector]  public double[] fitnesses;   //fitness values for all species
    Tuple<NDArray[], NDArray[]>[] neuralNetworks;

    //ensures that only the first instance of this class exists even when the scene is reloaded
    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this){
            Destroy(this.gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start(){
        noOfLayers = sizes.Length - 1;
        speciesAlive = noOfSpecies;
        fitnesses = new double[noOfSpecies];
        neuralNetworks = new Tuple<NDArray[], NDArray[]>[noOfSpecies];
        for (int netIndex = 0; netIndex < noOfSpecies; ++netIndex){
            neuralNetworks[netIndex] = new Tuple<NDArray[], NDArray[]>(new NDArray[noOfLayers], new NDArray[noOfLayers]); 
            for (int layer = 1; layer < noOfLayers + 1; ++layer){
                neuralNetworks[netIndex].Item1[layer - 1] = np.random.uniform(-randomRange, randomRange, new int[] { sizes[layer], sizes[layer - 1] });
                neuralNetworks[netIndex].Item2[layer - 1] = np.random.uniform(-randomRange, randomRange, new int[] { sizes[layer] });
            }
        }
    }

    //returns whether to flap or not
    //inputs include the height of the bird, distance to the next pole, and the height to the top and bottom of the gap
    public bool FeedForward(int netIndex, NDArray inputs){
        //z = wa + b
        //a = activ(z)
        NDArray[] activations = new NDArray[noOfLayers + 1];
        activations[0] = inputs;
        for(int layer = 1; layer < noOfLayers; ++layer)
            activations[layer] = Relu(np.dot(neuralNetworks[netIndex].Item1[layer-1], activations[layer-1]) + neuralNetworks[netIndex].Item2[layer-1]);
        double outputActivation = Sigmoid(np.dot(neuralNetworks[netIndex].Item1[noOfLayers - 1], activations[noOfLayers - 1]) + neuralNetworks[netIndex].Item2[noOfLayers - 1])[0];
        return outputActivation > 0.5;
    }

    //creates the next generation of species
    public void Restart(){
        //write weights and biases of fittest species to file
        string path = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\NeuralNetworks.csv");
        if (!File.Exists(path)){
            using (StreamWriter stream = File.CreateText(path))
                stream.Write(GenerateString());
        }
        else {
            using (StreamWriter stream = File.AppendText(path))
                stream.Write(GenerateString());
        }

        Random rnd = new Random();

        //whether to mutate or not. Chance of 50% when the highest score is 0 and 1% when the highest score is 50
        int probabilityOfMutation = Convert.ToInt32(Math.Ceiling(50 * Math.Exp(-0.1 * highestScore)));
        
        //selection
        double maxFitness = fitnesses.Max();
        List<Tuple<NDArray[], NDArray[]>> fittestSpecies = new List<Tuple<NDArray[], NDArray[]>>();
        for (int i = 0; i < noOfSpecies; ++i){
            if (fitnesses[i] > maxFitness - fitnessThreshold)
                fittestSpecies.Add(neuralNetworks[i]);
        }

        //preserve the fittest species for the next generation
        for(int i = 0; i < fittestSpecies.Count; ++i){
            neuralNetworks[i] = fittestSpecies[i];
        }

        //cross breeding and mutation
        for (int i = fittestSpecies.Count; i < noOfSpecies; ++i){
            NDArray[] weights = new NDArray[noOfLayers], biases = new NDArray[noOfLayers];
            for(int layer = 0; layer < noOfLayers; ++layer){
                NDArray weightMatrix = np.zeros(new int[] { sizes[layer+1], sizes[layer] });
                NDArray biasVector = np.zeros(new int[] { sizes[layer+1] });
                for (int row = 0; row < sizes[layer+1]; ++row){
                    for (int column = 0; column < sizes[layer]; ++column){
                        weightMatrix[row][column] = fittestSpecies[rnd.Next(fittestSpecies.Count)].Item1[layer][row][column];
                    }
                }
                for (int element = 0; element < sizes[layer+1]; ++element){
                    biasVector[element] = fittestSpecies[rnd.Next(fittestSpecies.Count)].Item2[layer][element];
                }
                weights[layer] = (rnd.Next(100) <= probabilityOfMutation) ? Shuffle(weightMatrix.flatten()).reshape(new int[] { sizes[layer+1], sizes[layer] }): weightMatrix;
                biases[layer] = (rnd.Next(100) <= probabilityOfMutation)? Shuffle(biasVector) : biasVector;
            }
            neuralNetworks[i] = Tuple.Create(weights, biases);
        }
        
        speciesAlive = noOfSpecies;
        fitnesses = new double[noOfSpecies];
        generation++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //swaps random elements of array
    NDArray Shuffle(NDArray arr){
        Random rnd = new Random();
        for(int indexOfElementToSwap = 0; indexOfElementToSwap < arr.size; ++indexOfElementToSwap){ 
            //12% probability that element will be swapped with another
            if (rnd.Next(100) < 12){
                int indexOfElementToSwapWith = rnd.Next(arr.size);
                Tuple<double, double> initial = new Tuple<double, double>(arr[indexOfElementToSwap], arr[indexOfElementToSwapWith]);
                arr[indexOfElementToSwap] = initial.Item2;
                arr[indexOfElementToSwapWith] = initial.Item1;
            }
        }
        return arr;
    }

    //relu activation function
    static NDArray Relu(NDArray z){
        return np.maximum(0.0, z);
    }

    //sigmoid activation function
    static NDArray Sigmoid(NDArray z){
        return 1 / (1 + np.exp(-z));
    }

    //creates string with weights and biases for the fittest species
    string GenerateString(){
        var bestSpecies = neuralNetworks[Array.IndexOf(fitnesses, fitnesses.Max())];
        string str = "Score:," + highestScore + "\n\n";
        int weightIndex = 0;
        foreach(var weightMatrix in bestSpecies.Item1){
            weightIndex++;
            str += (weightIndex == bestSpecies.Item1.Length - 1) ?  ",,Weights in Hidden Layer " + weightIndex + ":\n" : ",,Weights in Output Layer:\n";
            for(int row = 0; row < weightMatrix.Shape.Dimensions[0]; ++row){
                str += ",,";
                for(int column = 0; column < weightMatrix.Shape.Dimensions[1]; ++column){
                    double val = weightMatrix[row][column];
                    str += val + ",";
                }
                str += "\n";
            }
            str += "\n";
        }
        int biasIndex = 0;
        foreach (var biasVector in bestSpecies.Item2){
            biasIndex++;
            str += (biasIndex == bestSpecies.Item2.Length - 1) ?  ",,Biases in Hidden Layer " + biasIndex + ":\n" : ",,Biases in Output Layer:\n";
            str += ",,";
            for (int element = 0; element < biasVector.size; ++element){
                double val = biasVector[element];
                str += val + ",";
            }
            str += "\n\n";
        }
        return str;
    }
}
