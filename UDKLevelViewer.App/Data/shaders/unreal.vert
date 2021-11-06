#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 model;
uniform mat4 viewMatrix;
uniform mat4 projMatrix;

void main(void)
{
    texCoord = aTexCoord;
    gl_Position = vec4(aPosition, 1.0) * model * viewMatrix * projMatrix;
}