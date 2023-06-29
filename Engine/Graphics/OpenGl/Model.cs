﻿using Silk.NET.Assimp;
using Silk.NET.OpenGL;

using AssMesh = Silk.NET.Assimp.Mesh;

namespace Prospect.Engine.OpenGl;

class Model : IDisposable {
	public IReadOnlyList<Mesh> Meshes => _meshes;

	readonly GL _gl;
	readonly Assimp _assimp;

	readonly List<Mesh> _meshes = new();
	readonly Texture _texture;

	public unsafe Model( GL gl, string path, Texture texture ) {
		_gl = gl;
		_assimp = Assimp.GetApi();
		_texture = texture;

		var scene = _assimp.ImportFile( path, (uint)PostProcessSteps.Triangulate );
		if ( scene == null || scene->MFlags == Assimp.SceneFlagsIncomplete || scene->MRootNode == null )
			throw new Exception( _assimp.GetErrorStringS() );

		processNode( scene->MRootNode, scene );
	}

	unsafe void processNode( Node* node, Scene* scene ) {
		for ( var i = 0; i < node->MNumMeshes; i++ ) {
			var mesh = scene->MMeshes[node->MMeshes[i]];
			_meshes.Add( processMesh( mesh, scene ) );
		}

		for ( var i = 0; i < node->MNumChildren; i++ )
			processNode( node->MChildren[i], scene );
	}

	unsafe Mesh processMesh( AssMesh* mesh, Scene* scene ) {
		List<Vertex> vertices = new();
		List<uint> indices = new();

		// Walk through each of the mesh's vertices
		for ( uint i = 0; i < mesh->MNumVertices; i++ ) {
			Vertex vertex = new() {
				BoneIds = new int[Vertex.MAX_BONE_INFLUENCE],
				Weights = new float[Vertex.MAX_BONE_INFLUENCE],

				Position = mesh->MVertices[i]
			};

			// Normals
			if ( mesh->MNormals != null )
				vertex.Normal = mesh->MNormals[i];
			// Tangent
			if ( mesh->MTangents != null )
				vertex.Tangent = mesh->MTangents[i];
			// BiTangent
			if ( mesh->MBitangents != null )
				vertex.BiTangent = mesh->MBitangents[i];

			// Texture coordinates
			if ( mesh->MTextureCoords[0] != null ) // Does the mesh contain texture coordinates?
			{
				// A vertex can contain up to 8 different texture coordinates. We thus make the assumption that we won't 
				// use models where a vertex can have multiple texture coordinates so we always take the first set (0).
				Vector3f texcoord3 = mesh->MTextureCoords[0][i];
				vertex.TexCoords = new Vector2f( texcoord3.X, texcoord3.Y );
			}

			vertices.Add( vertex );
		}

		// Go thru all faces, grab their indices
		for ( uint i = 0; i < mesh->MNumFaces; i++ ) {
			var face = mesh->MFaces[i];
			for ( uint j = 0; j < face.MNumIndices; j++ )
				indices.Add( face.MIndices[j] );
		}

		return new( _gl, buildVertices( vertices ), indices.ToArray(), _texture );
	}

	float[] buildVertices( List<Vertex> vertexList ) {
		float[] vertices = new float[vertexList.Count * 5];

		for ( int i = 0; i < vertexList.Count; i++ ) {
			// There are 5 floats per vertex
			var vi = i * 5;
			var vertex = vertexList[i];

			vertices[vi] = vertex.Position.X;
			vertices[vi + 1] = vertex.Position.Y;
			vertices[vi + 2] = vertex.Position.Z;
			vertices[vi + 3] = vertex.TexCoords.X;
			vertices[vi + 4] = vertex.TexCoords.Y;
		}

		return vertices;
	}

	public void Dispose() {
		foreach ( var mesh in _meshes )
			mesh.Dispose();

		_meshes.Clear();
	}
}
