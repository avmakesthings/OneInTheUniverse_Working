using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeNeuron : MonoBehaviour
{
    /*
     * Neuron making code based on the "TREES Toolbox" algorithm https://www.treestoolbox.org/
       https://journals.plos.org/ploscompbiol/article?id=10.1371/journal.pcbi.1000877
       */
    static Material lineMaterial;

    TreeNode<NodeItem> neuron;

    //--------------------------------------------
    //  Start is called before the first frame update
    //--------------------------------------------

    void Start()
    {
        neuron = CreateNeuron(0.7f, 1000, 500, 1f, 0.33333333f);
        /* May want to tweak densprofindex; 1/3 gives a volume-uniform set of
         * carrier points. e.g. 1 would give more points near the center */
}

TreeNode<NodeItem> CreateNeuron(float bf, int num_cp, int num_links,
        float max_rad, float densprofindex)
    {

        //--- OjO: Start the tree here with origin at zeros
        Vector3 vec_origin = new Vector3(0f, 0f, 0f);

        //Assign random points
        double minscore = 1e30f;
        double dist2neuron_i = 0f;
        double totalscore = 0f;
        double EPSILON = 1e-6f;
        bool breakfromtree;
        var carrierpoints = new Vector3[num_cp];

        // Does the neuron include cp i?
        bool[] inneuron = new bool[num_cp];
        //What is a cp's parent?

        TreeNode<NodeItem> root = new TreeNode<NodeItem>(new NodeItem(0, vec_origin, vec_origin, 0f));

        //The soma point is in the neuron
        // Branching parameter bf: main "One Rule to Make them All" parameter

        /* Locate the carrier points*/
        inneuron[0] = true;
        carrierpoints[0] = vec_origin;
        for (int i = 1; i < num_cp; i++)
        {
            carrierpoints[i] = get_random(0, max_rad, densprofindex);
            inneuron[i] = false;
        }

        /* Make links */
        for (int ilink = 0; ilink < num_links; ilink++)
        {
            minscore = 1e30;

            // Find the closest cp to a node in the tree
            foreach (TreeNode<NodeItem> node in root)
            {
             
               for (int iout = 0; iout < num_cp; iout++)
                {
                    if (inneuron[iout] == false)
                    {
                        dist2neuron_i = (node.Data.VEC - carrierpoints[iout]).magnitude;
                        totalscore = dist2neuron_i + bf * node.Data.DISTINNEURON;
                        /* weight the score by the "balancing factor" bf,
                           including the distance from root to node within the neuron
                           Can add other weights, e.g. "gravitational potential" */
                        if (totalscore < minscore)
                        {
                            minscore = totalscore;
                        }
                    }
                }
            }
            
            /* Birth the child to the closest node (parent)
               It should be possible to do this without repeating the loop
               but I'm not sure how, with the node iterator */
            foreach (TreeNode<NodeItem> node in root)
            {
                breakfromtree = false;
                for (int iout = 0; iout < num_cp; iout++)
                {
                    if (inneuron[iout] == false)
                    {
                        dist2neuron_i = (node.Data.VEC - carrierpoints[iout]).magnitude;
                        totalscore = dist2neuron_i + bf * node.Data.DISTINNEURON;
                        if (Math.Abs(totalscore - minscore) < EPSILON)
                        {
                            TreeNode<NodeItem> child_i = node.AddChild(new NodeItem(iout, node.Data.VEC, carrierpoints[iout],
                                                                       node.Data.DISTINNEURON + dist2neuron_i));
                            inneuron[iout] = true;
                            breakfromtree = true;
                            break;
                        }
                    }
                }
                if (breakfromtree)
                {
                    break;
                }
            }
        }
        return root;
    }

    private TreeNode<T> TreeNode<T>(T nodeItem)
    {
        throw new NotImplementedException();
    }

    //--------------------------------------------
    // Will be called after all regular rendering is done
    //--------------------------------------------
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        renderNeuron();
    }

    //--------------------------------------------
    // Good old OpenGL direct mode...
    //--------------------------------------------
    void renderNeuron()
    {

        // Draw lines
        GL.Color(new Color(0.2f, 0.2f, 1.0f, 0.8f));
        GL.Begin(GL.LINES);

        //--- Here is where we extract all the nodels in the tree
        foreach (TreeNode<NodeItem> node in neuron)
        {
            GL.Vertex3(node.Data.PAR.x, node.Data.PAR.y, node.Data.PAR.z);
            GL.Vertex3(node.Data.VEC.x, node.Data.VEC.y, node.Data.VEC.z);
        }
        GL.End();
    }

    //--------------------------------------------
    //  Get a random position on the sphere over a range in radius
    //--------------------------------------------
    Vector3 get_random(float _low, float _high, float _densprofindex)
    {
        float _r = (float)Math.Pow(UnityEngine.Random.Range(_low, _high), _densprofindex);
        float _phi = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        float _costheta = UnityEngine.Random.Range(-1f, 1f);
        float _sintheta = Mathf.Sqrt(1f - _costheta * _costheta);
        float _x = _r * _sintheta * Mathf.Cos(_phi);
        float _y = _r * _sintheta * Mathf.Sin(_phi);
        float _z = _r * _costheta;

        return new Vector3(_x, _y, _z);
    }

    //--------------------------------------------
    //  Get a semi-random position on the sphere over a range in radius and angle form a reference vector
    //  I do this in a while loop which is very inneficient. It should be done correctly...
    //--------------------------------------------
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Built-in shader
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
        }
    }

}
