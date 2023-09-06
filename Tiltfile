docker_build(
  'ghcr.io/lancewells/henrymeds/myapp:latest-dev',
  '.'
)

k8s_yaml(
  './app.yaml',
)

k8s_resource(
  'myapp',
  port_forwards=['3000:80'],
)
