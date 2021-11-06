/*
@todo: Figure out how to possibly use UE3 materials directly. They have a bunch of vars which makes them weird to work with.
*/

#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D diffuse;
uniform sampler2D normal;
uniform sampler2D specular;

uniform float specIntensity;
uniform float specPower;

void main()
{
    outputColor = texture(diffuse, texCoord);
}