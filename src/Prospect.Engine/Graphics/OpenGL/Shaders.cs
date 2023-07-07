namespace Prospect.Engine.OpenGL;

static class Shaders {
	public const string VERTEX_SOURCE =
@"#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;
out vec2 frag_texCoords;
uniform mat4 uTransform;
uniform mat4 uView;
uniform mat4 uProjection;

void main() {
	gl_Position = uProjection * uView * uTransform * vec4(aPosition, 1.0);
	frag_texCoords = aTexCoords;
}";

	public const string FRAGMENT_SOURCE =
@"#version 330 core

in vec2 frag_texCoords;
out vec4 out_color;
uniform sampler2D uTexture;

void main() {
	out_color = texture(uTexture, frag_texCoords);
}";
}
