#version 330
in vec3 fragNormal;
out vec4 outputColor;

void main() {
    vec3 lightDir = normalize(vec3(1.0, 1.0, 1.0));
    float diff = max(dot(normalize(fragNormal), lightDir), 0.0);
    vec3 diffuse = diff * vec3(0.9, 0.9, 1.0);
    vec3 ambient = vec3(0.1, 0.0, 0.0);
    outputColor = vec4(ambient + diffuse, 1.0);
}